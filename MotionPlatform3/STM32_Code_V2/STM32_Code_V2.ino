// 3DOF by Andrey Zhuravlev
// Mofified for timer-based-stepping using ISR (increases speed/performance) by Laith Wattar
// Motion Smoothing Buffer implented starting on line 402 as recommended by Andrey Zhuravlev
// e-mail: v.azhure@gmail.com
// original version from 2025-06-21
// modified version from 2026-03-01
// modified version from 2026-03-10 : struct STATE speedMMperSEC uint8_t -> uint16_t. Overal length of struct not changed because of alignment
// stm32duino version

// NOTE: select fake STM32F103C8
// https://www.stm32duino.com/

//#define DEBUG
// Board: STM32F103C8T6 5 pcs (master + 4 slave)
// Upload: SWD
#define I2C_SDA PB7
#define I2C_SCL PB6
#include <Wire_slave.h>
#include <stdio.h>

// Uncomment this line if you using SF1610
#define SFU1610  // Used only in SLAVE devices

// Uncomment this line to flash MASTER device
// Comment this line to flash SLAVE devices
// #define I2CMASTER

// Maximal number of linear actuators
#define MAX_LINEAR_ACTUATORS 4
// Number of linear actuators
#define LINEAR_ACTUATORS MAX_LINEAR_ACTUATORS

#define SLAVE_ADDR_FL 10  // Front / Front Left
#define SLAVE_ADDR_RL 11  // Rear Left
#define SLAVE_ADDR_RR 12  // Rear Right
#define SLAVE_ADDR_FR 13  // Front RIGHT

#define SLAVE_FIRST SLAVE_ADDR_FL
#define SLAVE_LAST (SLAVE_FIRST + LINEAR_ACTUATORS - 1)

void allPinsPulldown() {
  for (int pin = PA0; pin <= PC15; pin++) pinMode(pin, INPUT_PULLDOWN);
}

inline int WireRead(uint8_t* ptr, uint8_t len) {
  int cnt = Wire.available();
  len = len > cnt ? cnt : len;
  for (int t = 0; t < len; t++) {
    ptr[t] = Wire.read();
  }
  return len;
}

template<class T>
const T& clamp(const T& x, const T& a, const T& b) {
  if (x < a) {
    return a;
  } else if (b < x) {
    return b;
  } else
    return x;
}

#ifndef I2CMASTER
/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Uncomment a single line with desired Address to flash SLAVE device
//#define SLAVE_ADDR SLAVE_ADDR_FL
//#define SLAVE_ADDR SLAVE_ADDR_RL
//#define SLAVE_ADDR SLAVE_ADDR_RR
#define SLAVE_ADDR SLAVE_ADDR_FR
#endif

#define SERIAL_BAUD_RATE 115200

// override Serial Port buffer size
#define SERIAL_TX_BUFFER_SIZE 512
#define SERIAL_RX_BUFFER_SIZE 512

#define LED_PIN PC13  // * Check your board. Some have PB2 or another (label near LED).

enum MODE : uint8_t { UNKNOWN,
                      CONNECTED,
                      DISABLED,
                      HOMEING,
                      PARKING,
                      READY,
                      ALARM
};

enum COMMAND : uint8_t { CMD_HOME,
                         CMD_MOVE,
                         CMD_SET_SPEED,
                         CMD_DISABLE,
                         CMD_ENABLE,
                         CMD_GET_STATE,
                         CMD_CLEAR_ALARM,
                         CMD_PARK,
                         SET_ALARM,
                         CMD_SET_SLOW_SPEED,
                         CMD_SET_ACCEL,
                         CMD_MOVE_SH
};

enum FLAGS : uint8_t { NONE = 0,
                       STATE_ON_LIMIT_SWITCH = 1,
                       STATE_HOMED = 1 << 1,
};

struct STATE {
  MODE mode;
  FLAGS flags;
  uint16_t speedMMperSEC;
  int32_t currentpos;
  int32_t targetpos;
  int32_t min;
  int32_t max;
};

const int STATE_LEN = sizeof(STATE);

struct PCCMD {
  uint8_t header = 0;
  uint8_t len;  // len
  COMMAND cmd;
  uint8_t reserved;
  int32_t data[MAX_LINEAR_ACTUATORS];
} pccmd;

// FOR SimHub
struct PCCMD_SH {
  uint8_t header = 0;
  uint8_t len;  // len
  COMMAND cmd;
  uint8_t reserved;
  uint16_t data[MAX_LINEAR_ACTUATORS];
  uint16_t data2[MAX_LINEAR_ACTUATORS];  // EMPTY
};

PCCMD_SH& pccmd_sh = *(PCCMD_SH*)&pccmd;

const int RAW_DATA_LEN = sizeof(PCCMD);

#ifdef I2CMASTER  // MASTER CODE below

#define ALARM_PIN PA4
#define SIGNAL_PIN PA5

// Device states
STATE st[MAX_LINEAR_ACTUATORS];
bool arr_slaves[MAX_LINEAR_ACTUATORS];

#define SET_SLAVE_STATE(idx, x) arr_slaves[idx] = x
#define HAS_SLAVE(idx) arr_slaves[idx]

volatile bool bAlarm = false;
volatile bool estopLatched = false;
uint32_t lastAlarmBroadcastMs = 0;

// Alarm input event
void OnAlarm() {
  bAlarm = true;
  estopLatched = true;
}

#define RETRY_CNT 3

void setup() {

  allPinsPulldown();

  pinMode(LED_PIN, OUTPUT);
  pinMode(ALARM_PIN, INPUT_PULLDOWN);
  pinMode(SIGNAL_PIN, INPUT_PULLDOWN);
  digitalWrite(LED_PIN, HIGH);

  attachInterrupt(ALARM_PIN, OnAlarm, RISING);
  estopLatched = (digitalRead(ALARM_PIN) == HIGH);
  if (estopLatched) {
    bAlarm = true;
  }
  Wire.begin();
  // Wire.setClock(400000);  // Reverted due to connectivity issues: use default I2C clock
  delay(1000);
  int nFound = 0;
  for (int t = 0; t < RETRY_CNT; t++) {
    nFound = 0;
    // scanning slave devices
    for (int addr = SLAVE_FIRST; addr <= SLAVE_LAST; addr++) {
      Wire.beginTransmission(addr);
      uint8_t err = Wire.endTransmission();
      SET_SLAVE_STATE(addr - SLAVE_FIRST, err == 0);
      if (err == 0) nFound++;
    }
    if (nFound == MAX_LINEAR_ACTUATORS)
      break;
    delay(100);
  }

  digitalWrite(LED_PIN, nFound == 0 ? HIGH : LOW);

  Serial.begin(SERIAL_BAUD_RATE);
  while (!Serial)
    ;  // whait connected

#ifdef DEBUG
  // I2C Devices scan
  Serial.println("Seeking slave devices");
  for (int addr = SLAVE_FIRST; addr <= SLAVE_LAST; addr++)
    for (int t = 0; t < RETRY_CNT; t++) {
      Wire.beginTransmission(addr);
      uint8_t err = Wire.endTransmission();
      if (err == 0) {
        Serial.print("Found Device: ");
        Serial.println(addr);
        break;
      } else if (err == 4) {
        Serial.print("Unknown error at address ");
        Serial.println(addr);
      }
      delay(500);
    }
  Serial.println("Seeking finished");
#endif
}

#define CMD_ID 0
// serial input buffer
uint8_t buf[RAW_DATA_LEN * 2];
int offset = 0;
volatile bool _bDataPresent = false;
unsigned long _lasttime;

void serialEvent() {
  int data_cnt = min(Serial.available(), RAW_DATA_LEN);
  if (data_cnt < 2)
    return;
  for (int t = 0; t < data_cnt; ++t) {
    int byte = Serial.read();
    if (offset > 0) {
      buf[offset++] = byte;
      if (offset == RAW_DATA_LEN) {
        memcpy(&pccmd, buf, RAW_DATA_LEN);
        _bDataPresent = true;
        _lasttime = millis();
        offset = 0;
      }
    } else {
      if (byte == CMD_ID) {
        if (Serial.peek() == RAW_DATA_LEN) {
          buf[offset++] = CMD_ID;
        }
      }
    }
  }
}

bool RequestSlaveState(uint8_t addr, uint8_t* ptr, uint8_t len) {
  if (HAS_SLAVE(addr - SLAVE_FIRST) && Wire.requestFrom(addr, len) == len && WireRead(ptr, len) == len) {
    return true;
  }
  return false;
}

bool TransmitCMD(uint8_t addr, uint8_t cmd, uint32_t data) {
  if (HAS_SLAVE(addr - SLAVE_FIRST)) {
    Wire.beginTransmission(addr);
    Wire.write(cmd);
    Wire.write((uint8_t*)&data, sizeof(uint32_t));
    return Wire.endTransmission() == 0;
  }
  return false;
}

void loop() {
  bool estopActive = (digitalRead(ALARM_PIN) == HIGH);
  if (estopActive) {
    estopLatched = true;
  }

  if (bAlarm || (estopActive && (millis() - lastAlarmBroadcastMs) > 100)) {
    for (int addr = SLAVE_FIRST; addr <= SLAVE_LAST; addr++)
      TransmitCMD(addr, COMMAND::SET_ALARM, 1);
    bAlarm = false;
    lastAlarmBroadcastMs = millis();
  }

  if (Serial.available())
    serialEvent();

  if (_bDataPresent) {
    digitalWrite(LED_PIN, LOW);  // Turn LED on
    switch (pccmd.cmd) {
      case COMMAND::CMD_HOME:
      case COMMAND::CMD_ENABLE:
      case COMMAND::CMD_DISABLE:
        {
          // While E-stop is latched, block all motion/state-changing commands.
          if (estopLatched) {
            break;
          }
          for (int t = 0; t < LINEAR_ACTUATORS; t++) {
            if (pccmd.data[t] == 1)
              TransmitCMD(SLAVE_FIRST + t, pccmd.cmd, 0);
          }
        }
        break;
      case COMMAND::CMD_CLEAR_ALARM:
        {
          // Require E-stop input release before allowing reset/clear.
          if (estopActive) {
            break;
          }
          for (int t = 0; t < LINEAR_ACTUATORS; t++) {
            if (pccmd.data[t] == 1)
              TransmitCMD(SLAVE_FIRST + t, pccmd.cmd, 0);
          }
          estopLatched = false;
        }
        break;
      case COMMAND::CMD_MOVE_SH:
        {
          if (estopLatched) {
            break;
          }
          //memcpy(&pccmd_sh, &pccmd, RAW_DATA_LEN);
          for (int t = 0; t < LINEAR_ACTUATORS; t++) {
            uint16_t val = pccmd_sh.data[t];
            val = (val >> 8) | (val << 8);
            uint32_t target = map(val, 0, 65535, st[t].min, st[t].max);
            TransmitCMD(SLAVE_FIRST + t, COMMAND::CMD_MOVE, target);
          }
        }
        break;
      case COMMAND::CMD_PARK:
      case COMMAND::CMD_MOVE:
      case COMMAND::CMD_SET_SPEED:
      case COMMAND::CMD_SET_SLOW_SPEED:
      case COMMAND::CMD_SET_ACCEL:
        {
          if (estopLatched) {
            break;
          }
          for (int t = 0; t < LINEAR_ACTUATORS; t++)
            TransmitCMD(SLAVE_FIRST + t, pccmd.cmd, pccmd.data[t]);
        }
        break;
      case COMMAND::CMD_GET_STATE:
        {
          for (int t = 0; t < LINEAR_ACTUATORS; t++) {
            if (!RequestSlaveState(SLAVE_FIRST + t, (uint8_t*)&st[t], STATE_LEN))
              st[t].mode = MODE::UNKNOWN;
          }
          for (int t = 0; t < LINEAR_ACTUATORS; t++) {
            Serial.write(SLAVE_FIRST + t);
            Serial.write(STATE_LEN);
            Serial.write((uint8_t*)&st[t], STATE_LEN);
          }
        }
        break;
    }
    _bDataPresent = false;
    digitalWrite(LED_PIN, HIGH);  // Turn LED off
  }
}
#else  // SLAVE CODE below

volatile MODE mode;

const uint32_t dirPin = PA4;
const uint32_t stepPin = PA5;

const uint32_t STEP_PIN_BIT = stepPin - PA0;
const uint32_t DIR_PIN_BIT = dirPin - PA0;
const uint32_t limiterPinNO = PA6;
const uint8_t limiterPinNC = PA7;

#define ANALOG_INPUT_MAX 4095

volatile uint32_t accel = 25000;                               // Acceleration in mm/s² (25000 = 2.5g, 12000 would be gentler for 60Hz buffering --> adjust via desktop app)
#define STEPS_CONTROL_DIST STEPS_PER_REVOLUTIONS / 4  // Distance in steps

#ifdef SFU1610
const float MM_PER_REV = 10.0f;                                // distance in mm per revolution
const float MAX_REVOLUTIONS = 8.75;                            // maximum revolutions for ballscrew (adjust depending on custom length, 14 = 140mm)
const int32_t STEPS_PER_REVOLUTIONS = 1000;                    // Steps per revolution, decreased from 2000 to 1000 to accomadate slower STM32s
const int32_t SAFE_DIST_IN_STEPS = STEPS_PER_REVOLUTIONS / 4;  // Safe traveling distance in steps
#define MAX_SPEED_MM_SEC 400  // maximum speed mm/sec
#else
//1605 ballscrew:
const float MM_PER_REV = 5.0f;                                 // distance in mm per revolution
const float MAX_REVOLUTIONS = 17.5;                            // maximum revolutions
const int32_t STEPS_PER_REVOLUTIONS = 1000;                    // Steps per revolution
const int32_t SAFE_DIST_IN_STEPS = STEPS_PER_REVOLUTIONS / 2;  // Safe traveling distance in steps
#define MAX_SPEED_MM_SEC 120  // maximum speed mm/sec (can adjust only up to 200 for 1605 ballscrews due to physical limitations)
#endif

const int32_t RANGE = (int32_t)(MAX_REVOLUTIONS * STEPS_PER_REVOLUTIONS);  // Maximum traveling distance, steps
const int32_t MIN_POS = SAFE_DIST_IN_STEPS;                                // minimal controlled position, steps
const int32_t MAX_POS = RANGE - SAFE_DIST_IN_STEPS;                        // maximal controlled position, steps
const uint8_t HOME_DIRECTION = HIGH;

#define MIN_PULSE_DELAY 5  // Minimal pulse interval, us, may be limited by hardware.

#define MIN_REVERSE_DELAY 6  // Delay between DIR and STEP signal on direction change, us

#define MMPERSEC2DELAY(mmps) 1000000 / (STEPS_PER_REVOLUTIONS * mmps / MM_PER_REV)

#define MIN_SPEED_MM_SEC 10
#define SLOW_SPEED_MM_SEC 10
#define DEFAULT_SPEED_MM_SEC 90

#ifndef MAX
#define MAX(a, b) (a > b ? a : b)
#endif

#ifndef MIN
#define MIN(a, b) (a < b ? a : b)
#endif

const int HOMEING_PULSE_DELAY = MAX(MIN_PULSE_DELAY, (int)MMPERSEC2DELAY(MIN_SPEED_MM_SEC) - MIN_PULSE_DELAY);     // us
const int FAST_PULSE_DELAY = MAX(MIN_PULSE_DELAY, (int)MMPERSEC2DELAY(DEFAULT_SPEED_MM_SEC) - MIN_PULSE_DELAY);    // us
const int SLOW_PULSE_DELAY = MAX(FAST_PULSE_DELAY * 2, (int)MMPERSEC2DELAY(SLOW_SPEED_MM_SEC) - MIN_PULSE_DELAY);  // us

volatile int iFastPulseDelay = FAST_PULSE_DELAY;
volatile int iSlowPulseDelay = SLOW_PULSE_DELAY;
int iFastPulseDelayMM = MAX_SPEED_MM_SEC;

volatile int32_t targetPos = (MIN_POS + MAX_POS) / 2;
volatile uint8_t limitSwitchState = HIGH;
volatile bool bHomed = false;
volatile int32_t currentPos = 0;
volatile uint8_t currentDir = LOW;

volatile bool LimitChanged = true;
volatile uint32_t accelStartTime = 0;  // Track when acceleration started

// ========== TIMER-BASED STEPPING VARIABLES ==========
HardwareTimer stepTimer(2);  // Timer 2 for Roger Clark core
volatile bool steppingEnabled = false;
volatile bool stepPinState = false;         // Track STEP pin state for pulse generation
volatile uint32_t targetFrequency = 1000;   // Target step frequency in Hz
volatile uint32_t currentFrequency = 1000;  // Current step frequency in Hz
volatile uint32_t accelStepCount = 0;       // Steps taken for acceleration tracking
// ====================================================

// ========== BUFFER + SMOOTHING VARIABLES ==========
#define MOVE_BUFFER_SIZE 6  // Buffer for 60 Hz (~100ms lookahead)
volatile int32_t moveBuffer[MOVE_BUFFER_SIZE];  // Queue for target positions
volatile uint8_t bufferWriteIdx = 0;            // Index for writing new commands
volatile uint8_t bufferReadIdx = 0;             // Index for reading next target
volatile bool bufferHasData = false;            // Flag: is there data in buffer?

// Smoothing factor for exponential interpolation (0.1 = very smooth, 0.9 = fast)
const float SMOOTH_FACTOR = 0.4f;      // Tuned for 60 Hz from SimHub
volatile float smoothedTargetPos = 0;  // Current smoothed intermediate target
// ====================================================

// Convert mm/sec to step frequency (Hz)
inline uint32_t mmPerSecToFreq(uint32_t mmPerSec) {
  return (uint32_t)((float)STEPS_PER_REVOLUTIONS * (float)mmPerSec / MM_PER_REV);
}

// Convert delay (us) to frequency (Hz)
inline uint32_t delayToFreq(uint32_t delayUs) {
  if (delayUs < MIN_PULSE_DELAY) delayUs = MIN_PULSE_DELAY;
  return 1000000 / (delayUs + MIN_PULSE_DELAY);
}

// ========== TIMER ISR - GENERATES STEP PULSES ==========
void timerISR() {
  // Check if we should stop stepping
  if (!steppingEnabled || currentPos == targetPos) {
    steppingEnabled = false;
    GPIOA->regs->BSRR = (1 << (STEP_PIN_BIT + 16));  // Ensure STEP is LOW
    stepPinState = false;
    return;
  }

  // Check limit switch - stop if on limit and moving towards it
  if (limitSwitchState == HIGH && currentDir == HOME_DIRECTION) {
    steppingEnabled = false;
    GPIOA->regs->BSRR = (1 << (STEP_PIN_BIT + 16));  // Ensure STEP is LOW
    stepPinState = false;
    return;
  }

  // Alternate between HIGH and LOW to create proper pulse width
  if (!stepPinState) {
    // Rising edge - set STEP HIGH
    GPIOA->regs->BSRR = (1 << STEP_PIN_BIT);
    stepPinState = true;
  } else {
    // Falling edge - set STEP LOW and update position
    GPIOA->regs->BSRR = (1 << (STEP_PIN_BIT + 16));
    stepPinState = false;

    // Update position on falling edge (one complete step)
    if (currentDir == HIGH) {
      currentPos++;
    } else {
      currentPos--;
    }
    accelStepCount++;
  }
}

// ========== INITIALIZE TIMER ==========
void initStepTimer() {
  stepTimer.pause();
  stepTimer.setPrescaleFactor(1);  // 72MHz / 1 = 72MHz timer clock
  stepTimer.setOverflow(36000);    // Initial value, will be updated dynamically
  stepTimer.setMode(TIMER_CH1, TIMER_OUTPUT_COMPARE);
  stepTimer.attachInterrupt(TIMER_CH1, timerISR);
  stepTimer.refresh();
  stepTimer.resume();
}

// ========== SET STEP FREQUENCY ==========
void setStepFrequency(uint32_t freqHz) {
  if (freqHz < 100) freqHz = 100;      // Minimum 100 Hz
  if (freqHz > 40000) freqHz = 40000;  // Maximum 40 kHz

  currentFrequency = freqHz;

  // Timer must run at 2x frequency since we alternate HIGH/LOW
  // Each step requires 2 ISR calls: one for HIGH, one for LOW
  uint32_t timerFreq = freqHz * 2;

  // Calculate overflow value: 72,000,000 / (frequency * 2)
  uint32_t overflow = 72000000UL / timerFreq;
  stepTimer.setOverflow(overflow);
}

// ========== START STEPPING ==========
void startStepping(uint8_t dir) {
  if (currentDir != dir) {
    // Direction change - add delay
    steppingEnabled = false;
    GPIOA->regs->BSRR = (1 << (STEP_PIN_BIT + 16));  // Ensure STEP is LOW
    stepPinState = false;
    delayMicroseconds(MIN_REVERSE_DELAY);

    // Set direction pin using direct GPIO
    if (dir == HIGH) {
      GPIOA->regs->BSRR = (1 << DIR_PIN_BIT);
    } else {
      GPIOA->regs->BSRR = (1 << (DIR_PIN_BIT + 16));
    }

    currentDir = dir;
    accelStepCount = 0;
  }

  stepPinState = false;  // Always start with LOW state
  steppingEnabled = true;
}

// ========== STOP STEPPING ==========
void stopStepping() {
  steppingEnabled = false;
  GPIOA->regs->BSRR = (1 << (STEP_PIN_BIT + 16));  // Ensure STEP is LOW when stopped
  stepPinState = false;
}

// ========== LEGACY BLOCKING STEP (UNUSED, KEPT FOR REFERENCE) ==========
// This function is no longer called - replaced by timer ISR
/*
inline void Step(uint8_t dir, int delay) {
  if (limitSwitchState == HIGH && dir == HOME_DIRECTION)
    return;  // ON LIMIT SWITCH

  digitalWrite(dirPin, dir);
  if (_oldDir != dir) {
    last_step_time = 0;
    delayMicroseconds(MIN_REVERSE_DELAY);
    _oldDir = dir;
  }

  double delta = MIN(MAX((micros() - last_step_time), 1), iSlowPulseDelay);

  double acc_pulse = pulse - (double)accel * delta / 1000000.0;
  pulse = acc_pulse > delay ? acc_pulse : delay;

  last_step_time = micros();
  digitalWrite(stepPin, HIGH);
  delayMicroseconds(MIN_PULSE_DELAY);
  digitalWrite(stepPin, LOW);
  delayMicroseconds((uint32_t)pulse);
  currentPos += dir == HIGH ? 1 : -1;
}
*/

// Limit switch event
void OnLimitSwitch() {
  // Refresh immediately so timerISR() sees the latest switch state.
  limitSwitchState = digitalRead(limiterPinNO);
  LimitChanged = true;
}

void receiveEvent(int size) {
  uint8_t cmd;
  uint32_t data;
  if (size >= 5 && Wire.readBytes(&cmd, 1) == 1 && Wire.readBytes((uint8_t*)&data, sizeof(uint32_t)) == sizeof(uint32_t)) {
    // While in ALARM, only allow clear alarm or reassert alarm commands.
    if (mode == MODE::ALARM && cmd != COMMAND::CMD_CLEAR_ALARM && cmd != COMMAND::SET_ALARM) {
      return;
    }
    switch (cmd) {
      case COMMAND::CMD_HOME:
        // Ignore repeated HOME when already homed and ready.
        if (mode == MODE::READY && bHomed) {
          break;
        }
        if (mode != MODE::HOMEING) {
          // Immediate homing entry: stop any active stepping before mode switch.
          stopStepping();
          setStepFrequency(delayToFreq(HOMEING_PULSE_DELAY));

          // Clear buffered/planned motion so old targets cannot resume.
          bufferWriteIdx = 0;
          bufferReadIdx = 0;
          bufferHasData = false;
          accelStepCount = 0;
          accelStartTime = 0;

          // Force homing direction output now.
          if (HOME_DIRECTION == HIGH) {
            GPIOA->regs->BSRR = (1 << DIR_PIN_BIT);
          } else {
            GPIOA->regs->BSRR = (1 << (DIR_PIN_BIT + 16));
          }
          currentDir = HOME_DIRECTION;

          currentPos = 0;
          smoothedTargetPos = currentPos;
          targetPos = RANGE * 1.2;
          bHomed = false;
          mode = MODE::HOMEING;
        }
        break;
      case COMMAND::CMD_PARK:
        if (bHomed) {
          mode = MODE::PARKING;
        }
        break;
      case COMMAND::CMD_MOVE:
        if (mode == MODE::READY) {
          // ✅ BUFFER IMPLEMENTATION: Write to buffer instead of direct targetPos
          int32_t clampedData = clamp((int32_t)data, (int32_t)MIN_POS, (int32_t)MAX_POS);

          // Check if buffer is not full
          uint8_t nextWriteIdx = (bufferWriteIdx + 1) % MOVE_BUFFER_SIZE;
          if (nextWriteIdx != bufferReadIdx) {
            moveBuffer[bufferWriteIdx] = clampedData;
            bufferWriteIdx = nextWriteIdx;
            bufferHasData = true;
          }
          // If buffer is full, skip command (prevents overflow)
        }
        break;
      case COMMAND::CMD_CLEAR_ALARM:
        {
          LimitChanged = true;
          // Reset buffer on enable
          bufferWriteIdx = 0;
          bufferReadIdx = 0;
          bufferHasData = false;
          smoothedTargetPos = currentPos;

          if (bHomed)
            mode = MODE::READY;
          else
            mode = MODE::CONNECTED;
        }
        break;
      case COMMAND::CMD_ENABLE:
        {
          LimitChanged = true;
          // Reset buffer on enable
          bufferWriteIdx = 0;
          bufferReadIdx = 0;
          bufferHasData = false;
          smoothedTargetPos = currentPos;

          if (bHomed)
            mode = MODE::READY;
          else
            mode = MODE::CONNECTED;
        }
        break;
      case COMMAND::CMD_DISABLE:
        mode = MODE::DISABLED;
        stopStepping();
        break;
      case COMMAND::CMD_SET_SPEED:
        iFastPulseDelayMM = data = clamp((int)data, MIN_SPEED_MM_SEC, MAX_SPEED_MM_SEC);
        iFastPulseDelay = MAX(MIN_PULSE_DELAY, (int)MMPERSEC2DELAY(data) - MIN_PULSE_DELAY);  // us
        break;
      case COMMAND::CMD_SET_SLOW_SPEED:
        iSlowPulseDelay = MAX(iFastPulseDelay, (int)MMPERSEC2DELAY(data) - MIN_PULSE_DELAY);  // us
        break;
      case COMMAND::CMD_SET_ACCEL:
        accel = data;
        break;
      case COMMAND::SET_ALARM:
        bHomed = false;
        mode = MODE::ALARM;
        stopStepping();
        break;
    }
  }
}

void requestEvent() {
  if (mode == MODE::UNKNOWN)
    mode = MODE::CONNECTED;
  STATE state = { mode, (FLAGS)((limitSwitchState == HIGH ? FLAGS::STATE_ON_LIMIT_SWITCH : 0) | (bHomed ? FLAGS::STATE_HOMED : 0)), (uint16_t)iFastPulseDelayMM, currentPos, targetPos, MIN_POS, MAX_POS };
  Wire.write((uint8_t*)&state, STATE_LEN);
}

// Initialization
void setup() {
  allPinsPulldown();

  pinMode(stepPin, OUTPUT);
  pinMode(dirPin, OUTPUT);
  pinMode(limiterPinNO, INPUT_PULLDOWN);
  pinMode(limiterPinNC, INPUT_PULLDOWN);
  pinMode(LED_PIN, OUTPUT);

  digitalWrite(stepPin, LOW);
  digitalWrite(dirPin, LOW);

  targetPos = (MIN_POS + MAX_POS) / 2;

  limitSwitchState = digitalRead(limiterPinNO);

  attachInterrupt(limiterPinNO, OnLimitSwitch, CHANGE);

  // Initialize timer-based stepping
  initStepTimer();

  Wire.begin(SLAVE_ADDR);
  // Wire.setClock(400000);  // Reverted due to connectivity issues: use default I2C clock
  Wire.onReceive(receiveEvent);
  Wire.onRequest(requestEvent);

  mode = MODE::UNKNOWN;
  LimitChanged = true;
  currentPos = 0;
  smoothedTargetPos = currentPos;  // Initialize smoothed position
}

// Main function
void loop() {
  if (LimitChanged) {
    LimitChanged = false;
    limitSwitchState = digitalRead(limiterPinNO);
    digitalWrite(LED_PIN, !limitSwitchState);
  }

  switch (mode) {
    case MODE::HOMEING:
      {
        if (limitSwitchState == HIGH)  // SITTING ON SWITCH
        {
          stopStepping();

          // Move away from switch - set target correctly based on direction
          if (HOME_DIRECTION == HIGH) {
            targetPos = currentPos - SAFE_DIST_IN_STEPS;  // Moving negative (away from switch)
          } else {
            targetPos = currentPos + SAFE_DIST_IN_STEPS;  // Moving positive (away from switch)
          }

          uint32_t homeFreq = delayToFreq(HOMEING_PULSE_DELAY);
          setStepFrequency(homeFreq);
          startStepping(!HOME_DIRECTION);

          // Wait based on direction
          if (HOME_DIRECTION == HIGH) {
            while (currentPos > targetPos) {
              if (mode == MODE::ALARM || !steppingEnabled) {
                break;
              }
              delay(1);
            }
            if (mode == MODE::ALARM || currentPos > targetPos) {
              mode = MODE::ALARM;
              bHomed = false;
              stopStepping();
              break;
            }
          } else {
            while (currentPos < targetPos) {
              if (mode == MODE::ALARM || !steppingEnabled) {
                break;
              }
              delay(1);
            }
            if (mode == MODE::ALARM || currentPos < targetPos) {
              mode = MODE::ALARM;
              bHomed = false;
              stopStepping();
              break;
            }
          }

          stopStepping();

          // Re-read limit switch state after backing off
          limitSwitchState = digitalRead(limiterPinNO);

          currentPos = MAX_POS;
          targetPos = (MIN_POS + MAX_POS) / 2;

          // Move to center at homing speed
          setStepFrequency(homeFreq);
          uint8_t centerDir = (targetPos > currentPos) ? HIGH : LOW;  // Calculate correct direction
          startStepping(centerDir);

          while (targetPos != currentPos) {
            if (mode == MODE::ALARM || !steppingEnabled) {
              break;
            }
            delay(1);
          }
          if (mode == MODE::ALARM || targetPos != currentPos) {
            mode = MODE::ALARM;
            bHomed = false;
            stopStepping();
            break;
          }

          stopStepping();
          smoothedTargetPos = currentPos;
          bufferWriteIdx = 0;
          bufferReadIdx = 0;
          bufferHasData = false;
          mode = MODE::READY;
          bHomed = true;
          break;
        }

        digitalWrite(LED_PIN, (millis() % 500) > 250 ? HIGH : LOW);

        if (abs(currentPos) >= RANGE * 1.2)  // SWITCH NOT FOUND
        {
          mode = MODE::ALARM;
          bHomed = false;
          stopStepping();
          break;
        }

        // Continue homing - only start stepping if not already stepping
        if (!steppingEnabled) {
          uint32_t homeFreq = delayToFreq(HOMEING_PULSE_DELAY);
          setStepFrequency(homeFreq);
          startStepping(HOME_DIRECTION);
        }
      }
      break;

    case MODE::PARKING:
      {
        stopStepping();
        targetPos = MIN_POS;
        uint32_t homeFreq = delayToFreq(HOMEING_PULSE_DELAY);
        setStepFrequency(homeFreq);

        uint8_t dir = (targetPos > currentPos) ? HIGH : LOW;
        startStepping(dir);

        while (targetPos != currentPos) {
          if (mode == MODE::ALARM || !steppingEnabled) {
            break;
          }
          delay(1);
        }
        if (mode == MODE::ALARM || targetPos != currentPos) {
          mode = MODE::ALARM;
          bHomed = false;
          stopStepping();
          break;
        }

        stopStepping();
        smoothedTargetPos = currentPos;
        mode = MODE::READY;
      }
      break;

    case MODE::READY:
      {
        // ✅ STEP 1: Buffer Management - Load next point from buffer
        if (bufferHasData && bufferReadIdx != bufferWriteIdx) {
          // Get next target from buffer
          int32_t nextBufferTarget = moveBuffer[bufferReadIdx];

          // Check if we are close enough to current target to switch to next
          // This ensures we don't change direction too frequently
          if (abs(targetPos - currentPos) < 100) {  // 100 steps threshold (~1mm)
            targetPos = nextBufferTarget;
            bufferReadIdx = (bufferReadIdx + 1) % MOVE_BUFFER_SIZE;
            accelStartTime = millis();  // Reset acceleration timer for new segment
          }
        }

        // ✅ STEP 2: Exponential Smoothing
        // Blend the targetPos to avoid sudden jumps in velocity calculation
        smoothedTargetPos = smoothedTargetPos + (targetPos - smoothedTargetPos) * SMOOTH_FACTOR;

        // ✅ STEP 3: Motion Control Logic
        if (abs((int32_t)smoothedTargetPos - currentPos) > 5) {  // 5 step deadzone

          // Calculate remaining distance to smoothed target
          long dist = constrain(abs((int32_t)smoothedTargetPos - currentPos), 0, STEPS_CONTROL_DIST);

          // Map distance to frequency (deceleration profile based on remaining distance)
          uint32_t minFreq = delayToFreq(iSlowPulseDelay);
          uint32_t maxFreq = delayToFreq(iFastPulseDelay);

          // Linear interpolation based on distance for deceleration
          uint32_t mappedFreq = map(dist, 0, STEPS_CONTROL_DIST, minFreq, maxFreq);

          // Apply TRUE time-based acceleration
          uint32_t elapsedMs = millis() - accelStartTime;
          float elapsedSec = elapsedMs / 1000.0f;

          // Frequency increase = (accel in mm/s²) * (steps per mm) * (time in sec)
          float freqIncrease = (float)accel * (STEPS_PER_REVOLUTIONS / MM_PER_REV) * elapsedSec;
          uint32_t accelFreq = minFreq + (uint32_t)freqIncrease;
          if (accelFreq > maxFreq) accelFreq = maxFreq;

          // Use the minimum of mapped (decel) and accel frequencies
          uint32_t finalFreq = (mappedFreq < accelFreq) ? mappedFreq : accelFreq;

          // Only update frequency if it changed significantly (avoid constant resets)
          if (abs((int32_t)finalFreq - (int32_t)currentFrequency) > 100) {
            setStepFrequency(finalFreq);
          }

          uint8_t dir = ((int32_t)smoothedTargetPos > currentPos) ? HIGH : LOW;

          // Only start stepping if not already stepping, or if direction changed
          if (!steppingEnabled || currentDir != dir) {
            accelStartTime = millis();  // Reset acceleration timer
            startStepping(dir);
          }
        } else {
          // Target reached
          stopStepping();
          accelStepCount = 0;

          // If buffer is empty and we reached target, sync smoothed position
          if (!bufferHasData || bufferReadIdx == bufferWriteIdx) {
            smoothedTargetPos = currentPos;
          }
        }
      }
      break;

    case MODE::ALARM:
      digitalWrite(LED_PIN, (millis() % 250) > 125 ? HIGH : LOW);
      stopStepping();
      break;

    case MODE::DISABLED:
      stopStepping();
      break;

    default:
      break;
  }
}
#endif
