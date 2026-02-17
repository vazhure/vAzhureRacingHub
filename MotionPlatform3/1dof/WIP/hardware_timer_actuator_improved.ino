// 3DOF by Andrey Zhuravlev - OPTIMIZED VERSION
// e-mail: v.azhure@gmail.com
// Optimized version with improved acceleration algorithm and performance
// stm32duino version

// IMPROVEMENTS MADE:
// 1. Proper trapezoidal acceleration profile (accel/cruise/decel)
// 2. Optimized timer ISR with reduced overhead
// 3. Non-blocking motion control (removed delay() loops)
// 4. Critical section protection for shared variables
// 5. Step count based acceleration (more accurate than time-based)
// 6. Reduced CPU load with smarter frequency updates
// 7. Proper ramp calculations using step counts

//#define INVERTED_DIR
//#define DEBUG

#define I2C_SDA PB7
#define I2C_SCL PB6
#include <Wire_slave.h>
#include <stdio.h>

#define SFU1610

//#define I2CMASTER

#define MAX_LINEAR_ACTUATORS 4
#define LINEAR_ACTUATORS MAX_LINEAR_ACTUATORS

#define SLAVE_ADDR_FL 10
#define SLAVE_ADDR_RL 11
#define SLAVE_ADDR_RR 12
#define SLAVE_ADDR_FR 13

#define SLAVE_FIRST SLAVE_ADDR_FL
#define SLAVE_LAST (SLAVE_FIRST + LINEAR_ACTUATORS - 1)

// Optimized read function
inline int WireRead(uint8_t* ptr, uint8_t len) {
  int cnt = Wire.available();
  len = len > cnt ? cnt : len;
  for (int t = 0; t < len; t++) {
    ptr[t] = Wire.read();
  }
  return len;
}

template<class T>
inline const T& clamp(const T& x, const T& a, const T& b) {
  return (x < a) ? a : ((b < x) ? b : x);
}

#ifndef I2CMASTER
//#define SLAVE_ADDR SLAVE_ADDR_FL
//#define SLAVE_ADDR SLAVE_ADDR_RL
//#define SLAVE_ADDR SLAVE_ADDR_RR
//#define SLAVE_ADDR SLAVE_ADDR_FR
#endif

#define SERIAL_BAUD_RATE 115200
#define SERIAL_TX_BUFFER_SIZE 512
#define SERIAL_RX_BUFFER_SIZE 512

#define LED_PIN PC13

enum MODE : uint8_t { UNKNOWN, CONNECTED, DISABLED, HOMEING, PARKING, READY, ALARM };

enum COMMAND : uint8_t { CMD_HOME, CMD_MOVE, CMD_SET_SPEED, CMD_DISABLE, CMD_ENABLE,
                         CMD_GET_STATE, CMD_CLEAR_ALARM, CMD_PARK, SET_ALARM,
                         CMD_SET_SLOW_SPEED, CMD_SET_ACCEL, CMD_MOVE_SH };

enum FLAGS : uint8_t { NONE = 0, STATE_ON_LIMIT_SWITCH = 1, STATE_HOMED = 1 << 1 };

struct STATE {
  MODE mode;
  FLAGS flags;
  uint8_t speedMMperSEC;
  int32_t currentpos;
  int32_t targetpos;
  int32_t min;
  int32_t max;
};

const int STATE_LEN = sizeof(STATE);

struct PCCMD {
  uint8_t header = 0;
  uint8_t len;
  COMMAND cmd;
  uint8_t reserved;
  int32_t data[MAX_LINEAR_ACTUATORS];
} pccmd;

struct PCCMD_SH {
  uint8_t header = 0;
  uint8_t len;
  COMMAND cmd;
  uint8_t reserved;
  uint16_t data[MAX_LINEAR_ACTUATORS];
  uint16_t data2[MAX_LINEAR_ACTUATORS];
};

PCCMD_SH& pccmd_sh = *(PCCMD_SH*)&pccmd;
const int RAW_DATA_LEN = sizeof(PCCMD);

#ifdef I2CMASTER

// ==================== MASTER CODE ====================

#define ALARM_PIN PA4
#define SIGNAL_PIN PA5

STATE st[MAX_LINEAR_ACTUATORS];
bool arr_slaves[MAX_LINEAR_ACTUATORS];

#define SET_SLAVE_STATE(idx, x) arr_slaves[idx] = x
#define HAS_SLAVE(idx) arr_slaves[idx]

volatile bool bAlarm = false;

void OnAlarm() {
  bAlarm = true;
}

void setup() {
  pinMode(LED_PIN, OUTPUT);
  pinMode(ALARM_PIN, INPUT_PULLDOWN);
  pinMode(SIGNAL_PIN, INPUT);
  digitalWrite(LED_PIN, HIGH);

  attachInterrupt(ALARM_PIN, OnAlarm, RISING);
  Wire.begin();
  delay(1000);
  
  int nFound = 0;
  for (int addr = SLAVE_FIRST; addr <= SLAVE_LAST; addr++) {
    Wire.beginTransmission(addr);
    uint8_t err = Wire.endTransmission();
    SET_SLAVE_STATE(addr - SLAVE_FIRST, err == 0);
    if (err == 0) nFound++;
  }
  digitalWrite(LED_PIN, nFound == 0 ? HIGH : LOW);

  Serial.begin(SERIAL_BAUD_RATE);
  while (!Serial);
}

#define CMD_ID 0
uint8_t buf[RAW_DATA_LEN * 2];
int offset = 0;
volatile bool _bDataPresent = false;
unsigned long _lasttime;

// Improved serial handling - process all available bytes
void serialEvent() {
  while (Serial.available() > 0) {
    int byte = Serial.read();
    if (offset > 0) {
      buf[offset++] = byte;
      if (offset == RAW_DATA_LEN) {
        memcpy(&pccmd, buf, RAW_DATA_LEN);
        _bDataPresent = true;
        _lasttime = millis();
        offset = 0;
      }
    } else if (byte == CMD_ID && Serial.peek() == RAW_DATA_LEN) {
      buf[offset++] = CMD_ID;
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
  if (bAlarm) {
    for (int addr = SLAVE_FIRST; addr <= SLAVE_LAST; addr++)
      TransmitCMD(addr, COMMAND::SET_ALARM, 1);
    bAlarm = false;
  }

  if (Serial.available())
    serialEvent();

  if (_bDataPresent) {
    digitalWrite(LED_PIN, LOW);
    switch (pccmd.cmd) {
      case COMMAND::CMD_HOME:
      case COMMAND::CMD_ENABLE:
      case COMMAND::CMD_DISABLE:
      case COMMAND::CMD_CLEAR_ALARM:
        for (int t = 0; t < LINEAR_ACTUATORS; t++) {
          if (pccmd.data[t] == 1)
            TransmitCMD(SLAVE_FIRST + t, pccmd.cmd, 0);
        }
        break;
      case COMMAND::CMD_MOVE_SH:
        for (int t = 0; t < LINEAR_ACTUATORS; t++) {
          uint16_t val = pccmd_sh.data[t];
          val = (val >> 8) | (val << 8);
          uint32_t target = map(val, 0, 65535, st[t].min, st[t].max);
          TransmitCMD(SLAVE_FIRST + t, COMMAND::CMD_MOVE, target);
        }
        break;
      case COMMAND::CMD_PARK:
      case COMMAND::CMD_MOVE:
      case COMMAND::CMD_SET_SPEED:
      case COMMAND::CMD_SET_SLOW_SPEED:
      case COMMAND::CMD_SET_ACCEL:
        for (int t = 0; t < LINEAR_ACTUATORS; t++)
          TransmitCMD(SLAVE_FIRST + t, pccmd.cmd, pccmd.data[t]);
        break;
      case COMMAND::CMD_GET_STATE:
        for (int t = 0; t < LINEAR_ACTUATORS; t++) {
          if (!RequestSlaveState(SLAVE_FIRST + t, (uint8_t*)&st[t], STATE_LEN))
            st[t].mode = MODE::UNKNOWN;
        }
        for (int t = 0; t < LINEAR_ACTUATORS; t++) {
          Serial.write(SLAVE_FIRST + t);
          Serial.write(STATE_LEN);
          Serial.write((uint8_t*)&st[t], STATE_LEN);
        }
        break;
    }
    _bDataPresent = false;
    digitalWrite(LED_PIN, HIGH);
  }
}

#else

// ==================== SLAVE CODE - OPTIMIZED ====================

volatile MODE mode;

#ifdef INVERTED_DIR
const uint32_t dirPin = PA5;
const uint32_t stepPin = PA4;
#else
const uint32_t dirPin = PA4;
const uint32_t stepPin = PA5;
#endif
const uint32_t limiterPinNO = PA6;
const uint8_t limiterPinNC = PA7;

#define ANALOG_INPUT_MAX 4095

#ifdef SFU1610
const float MM_PER_REV = 10.0f;
const float MAX_REVOLUTIONS = 14.0;
const int32_t STEPS_PER_REVOLUTIONS = 1000;
const int32_t SAFE_DIST_IN_STEPS = STEPS_PER_REVOLUTIONS / 4;
#define MAX_SPEED_MM_SEC 400
#else
const float MM_PER_REV = 5.0f;
const float MAX_REVOLUTIONS = 17.5;
const int32_t STEPS_PER_REVOLUTIONS = 1000;
const int32_t SAFE_DIST_IN_STEPS = STEPS_PER_REVOLUTIONS / 2;
#define MAX_SPEED_MM_SEC 120
#endif

// ==================== CONSTANTS ====================
const int32_t RANGE = (int32_t)(MAX_REVOLUTIONS * STEPS_PER_REVOLUTIONS);
const int32_t MIN_POS = SAFE_DIST_IN_STEPS;
const int32_t MAX_POS = RANGE - SAFE_DIST_IN_STEPS;
const uint8_t HOME_DIRECTION = HIGH;

#define MIN_PULSE_DELAY 5       // us, hardware limit
#define MIN_REVERSE_DELAY 6     // us, direction change delay

// Pre-calculated constant for speed conversion
#define STEPS_PER_MM ((float)STEPS_PER_REVOLUTIONS / MM_PER_REV)

// ==================== ACCELERATION SETTINGS ====================
// Default acceleration: 25000 mm/s² = 2.55g (adjust as needed)
volatile uint32_t accel = 25000;

// Acceleration ramp settings
#define ACCEL_RAMP_STEPS 200    // Steps to reach full speed at default acceleration
#define MIN_SPEED_MM_SEC 10
#define SLOW_SPEED_MM_SEC 10
#define DEFAULT_SPEED_MM_SEC 90

#define MMPERSEC2DELAY(mmps) (1000000 / (STEPS_PER_REVOLUTIONS * mmps / MM_PER_REV))

#ifndef MAX
#define MAX(a, b) ((a) > (b) ? (a) : (b))
#endif
#ifndef MIN
#define MIN(a, b) ((a) < (b) ? (a) : (b))
#endif

// Pre-calculated pulse delays
const int HOMEING_PULSE_DELAY = MAX(MIN_PULSE_DELAY, (int)MMPERSEC2DELAY(MIN_SPEED_MM_SEC) - MIN_PULSE_DELAY);
const int FAST_PULSE_DELAY = MAX(MIN_PULSE_DELAY, (int)MMPERSEC2DELAY(DEFAULT_SPEED_MM_SEC) - MIN_PULSE_DELAY);
const int SLOW_PULSE_DELAY = MAX(FAST_PULSE_DELAY * 2, (int)MMPERSEC2DELAY(SLOW_SPEED_MM_SEC) - MIN_PULSE_DELAY);

// ==================== MOTION STATE VARIABLES ====================
volatile int iFastPulseDelayMM = MAX_SPEED_MM_SEC;
volatile int iSlowPulseDelay = SLOW_PULSE_DELAY;
int iFastPulseDelayMM_Setting = MAX_SPEED_MM_SEC;

volatile int32_t targetPos = (MIN_POS + MAX_POS) / 2;
uint8_t limitSwitchState = HIGH;
volatile bool bHomed = false;

// Use atomic access for position tracking
volatile int32_t currentPos = 0;
volatile uint8_t currentDir = LOW;
volatile bool steppingEnabled = false;
volatile bool LimitChanged = true;

// ==================== TRAPEZOIDAL MOTION PROFILE ====================
// This provides smooth acceleration and deceleration
volatile int32_t motionStartPos = 0;      // Position when motion started
volatile int32_t motionTotalSteps = 0;    // Total steps to move
volatile int32_t motionAccelSteps = 0;    // Steps for acceleration phase
volatile int32_t motionDecelSteps = 0;    // Steps for deceleration phase
volatile uint32_t motionStartFreq = 100;  // Starting frequency (Hz)
volatile uint32_t motionMaxFreq = 1000;   // Maximum frequency (Hz)
volatile uint32_t motionCurrentFreq = 100; // Current frequency
volatile int32_t motionCurrentStep = 0;   // Current step in motion

// ==================== TIMER SETUP ====================
HardwareTimer stepTimer(2);
volatile bool stepPinState = false;

// Critical section macros for ARM Cortex-M3
#define ENTER_CRITICAL() uint32_t _primask = __get_PRIMASK(); __disable_irq()
#define EXIT_CRITICAL() __set_PRIMASK(_primask)

// ==================== FREQUENCY CONVERSION ====================
inline uint32_t mmPerSecToFreq(uint32_t mmPerSec) {
  return (uint32_t)(STEPS_PER_MM * (float)mmPerSec);
}

inline uint32_t delayToFreq(uint32_t delayUs) {
  if (delayUs < MIN_PULSE_DELAY) delayUs = MIN_PULSE_DELAY;
  return 1000000 / (delayUs + MIN_PULSE_DELAY);
}

// ==================== OPTIMIZED TIMER ISR ====================
void timerISR() {
  // Early exit if stepping disabled
  if (!steppingEnabled) {
    GPIOA->regs->BSRR = (1 << (5 + 16));  // STEP LOW
    stepPinState = false;
    return;
  }
  
  // Generate step pulse
  if (!stepPinState) {
    // Rising edge
    GPIOA->regs->BSRR = (1 << 5);  // STEP HIGH
    stepPinState = true;
  } else {
    // Falling edge - complete step
    GPIOA->regs->BSRR = (1 << (5 + 16));  // STEP LOW
    stepPinState = false;
    
    // Update position
    if (currentDir == HIGH) {
      currentPos++;
    } else {
      currentPos--;
    }
    motionCurrentStep++;
    
    // Check if target reached
    if (currentPos == targetPos) {
      steppingEnabled = false;
      return;
    }
    
    // Check limit switch (safety)
    if (limitSwitchState == HIGH && currentDir == HOME_DIRECTION) {
      steppingEnabled = false;
      return;
    }
    
    // ========== TRAPEZOIDAL MOTION PROFILE ==========
    // Calculate current frequency based on position in motion
    int32_t remaining = abs(targetPos - currentPos);
    int32_t completed = motionCurrentStep;
    
    uint32_t newFreq = motionCurrentFreq;
    
    if (completed < motionAccelSteps) {
      // Acceleration phase: linear ramp up
      // freq = startFreq + (maxFreq - startFreq) * (step / accelSteps)
      uint32_t accelProgress = (completed * (motionMaxFreq - motionStartFreq)) / motionAccelSteps;
      newFreq = motionStartFreq + accelProgress;
    } else if (remaining < motionDecelSteps) {
      // Deceleration phase: linear ramp down
      // freq = startFreq + (maxFreq - startFreq) * (remaining / decelSteps)
      uint32_t decelProgress = (remaining * (motionMaxFreq - motionStartFreq)) / motionDecelSteps;
      newFreq = motionStartFreq + decelProgress;
    } else {
      // Cruise phase: maintain max speed
      newFreq = motionMaxFreq;
    }
    
    // Apply frequency with hysteresis to reduce timer updates
    // Only update if change is significant (>5%)
    if (newFreq != motionCurrentFreq) {
      uint32_t diff = (newFreq > motionCurrentFreq) ? 
                      (newFreq - motionCurrentFreq) : 
                      (motionCurrentFreq - newFreq);
      if (diff > (motionCurrentFreq >> 4) || diff > 50) {  // >6.25% or >50Hz
        motionCurrentFreq = newFreq;
        uint32_t overflow = 72000000UL / (newFreq * 2);
        stepTimer.setOverflow(overflow);
      }
    }
  }
}

// ==================== MOTION PLANNING ====================
void calculateMotionProfile(int32_t from, int32_t to, uint32_t maxFreq, uint32_t accelMMperSec2) {
  motionTotalSteps = abs(to - from);
  motionStartPos = from;
  motionCurrentStep = 0;
  motionStartFreq = delayToFreq(SLOW_PULSE_DELAY);  // Start from slow speed
  motionMaxFreq = maxFreq;
  
  // Calculate steps needed for acceleration/deceleration
  // Using kinematic equation: v² = v₀² + 2*a*d
  // For trapezoidal profile: accel_steps = v_max² / (2 * a)
  // where v is in steps/sec, a is in steps/sec²
  
  // Convert acceleration from mm/s² to steps/s²
  float accelStepsPerSec2 = accelMMperSec2 * STEPS_PER_MM;
  
  // Calculate frequency change rate
  // From v = a*t, we get t = v/a
  // Distance during acceleration = 0.5 * a * t²
  // So: accel_steps = v² / (2 * a)
  float maxFreqF = (float)maxFreq;
  float accelSteps = (maxFreqF * maxFreqF) / (2.0f * accelStepsPerSec2);
  
  motionAccelSteps = (int32_t)accelSteps;
  motionDecelSteps = motionAccelSteps;
  
  // Handle short moves (triangular profile)
  if (motionAccelSteps + motionDecelSteps > motionTotalSteps) {
    // Can't reach max speed - use triangular profile
    motionAccelSteps = motionTotalSteps / 2;
    motionDecelSteps = motionTotalSteps - motionAccelSteps;
    
    // Recalculate max frequency for triangular profile
    // d = 0.5 * a * t², and v = a * t
    // So: v = sqrt(2 * a * d)
    maxFreqF = sqrtf(2.0f * accelStepsPerSec2 * (float)motionAccelSteps);
    motionMaxFreq = (uint32_t)maxFreqF;
    if (motionMaxFreq < motionStartFreq) {
      motionMaxFreq = motionStartFreq;
    }
  }
  
  motionCurrentFreq = motionStartFreq;
}

void initStepTimer() {
  stepTimer.pause();
  stepTimer.setPrescaleFactor(1);  // 72MHz timer clock
  stepTimer.setOverflow(36000);    // Default 1kHz
  stepTimer.setMode(TIMER_CH1, TIMER_OUTPUT_COMPARE);
  stepTimer.attachInterrupt(TIMER_CH1, timerISR);
  stepTimer.refresh();
  stepTimer.resume();
}

void setStepFrequency(uint32_t freqHz) {
  if (freqHz < 50) freqHz = 50;        // Minimum 50 Hz
  if (freqHz > 50000) freqHz = 50000;  // Maximum 50 kHz
  
  uint32_t overflow = 72000000UL / (freqHz * 2);
  
  ENTER_CRITICAL();
  stepTimer.setOverflow(overflow);
  motionCurrentFreq = freqHz;
  EXIT_CRITICAL();
}

void startStepping(uint8_t dir) {
  ENTER_CRITICAL();
  
  if (currentDir != dir) {
    // Direction change - ensure proper timing
    steppingEnabled = false;
    GPIOA->regs->BSRR = (1 << (5 + 16));  // STEP LOW
    stepPinState = false;
    
    // Set direction pin
    if (dir == HIGH) {
      GPIOA->regs->BSRR = (1 << 4);  // DIR HIGH
    } else {
      GPIOA->regs->BSRR = (1 << (4 + 16));  // DIR LOW
    }
    
    EXIT_CRITICAL();
    delayMicroseconds(MIN_REVERSE_DELAY);
    ENTER_CRITICAL();
    
    currentDir = dir;
    motionCurrentStep = 0;
  }
  
  stepPinState = false;
  
  // Calculate motion profile for new move
  if (currentPos != targetPos) {
    uint32_t maxFreq = delayToFreq(FAST_PULSE_DELAY);
    if (maxFreq > mmPerSecToFreq(iFastPulseDelayMM_Setting)) {
      maxFreq = mmPerSecToFreq(iFastPulseDelayMM_Setting);
    }
    calculateMotionProfile(currentPos, targetPos, maxFreq, accel);
    
    // Set initial frequency
    uint32_t overflow = 72000000UL / (motionStartFreq * 2);
    stepTimer.setOverflow(overflow);
  }
  
  steppingEnabled = true;
  EXIT_CRITICAL();
}

void stopStepping() {
  ENTER_CRITICAL();
  steppingEnabled = false;
  GPIOA->regs->BSRR = (1 << (5 + 16));  // STEP LOW
  stepPinState = false;
  EXIT_CRITICAL();
}

// ==================== LIMIT SWITCH ====================
void OnLimitSwitch() {
  LimitChanged = true;
}

// ==================== I2C HANDLERS ====================
void receiveEvent(int size) {
  uint8_t cmd;
  uint32_t data;
  if (size >= 5 && Wire.readBytes(&cmd, 1) == 1 && 
      Wire.readBytes((uint8_t*)&data, sizeof(uint32_t)) == sizeof(uint32_t)) {
    switch (cmd) {
      case COMMAND::CMD_HOME:
        if (mode != MODE::HOMEING) {
          mode = MODE::HOMEING;
          currentPos = 0;
          targetPos = RANGE * 1.2;
          bHomed = false;
        }
        break;
      case COMMAND::CMD_PARK:
        if (bHomed) {
          mode = MODE::PARKING;
        }
        break;
      case COMMAND::CMD_MOVE:
        if (mode == MODE::READY) {
          targetPos = clamp(data, (uint32_t)MIN_POS, (uint32_t)MAX_POS);
        }
        break;
      case COMMAND::CMD_CLEAR_ALARM:
      case COMMAND::CMD_ENABLE:
        LimitChanged = true;
        mode = bHomed ? MODE::READY : MODE::CONNECTED;
        break;
      case COMMAND::CMD_DISABLE:
        mode = MODE::DISABLED;
        stopStepping();
        break;
      case COMMAND::CMD_SET_SPEED:
        iFastPulseDelayMM_Setting = clamp((int)data, MIN_SPEED_MM_SEC, MAX_SPEED_MM_SEC);
        iFastPulseDelayMM = iFastPulseDelayMM_Setting;
        break;
      case COMMAND::CMD_SET_SLOW_SPEED:
        iSlowPulseDelay = MAX(iFastPulseDelay, (int)MMPERSEC2DELAY(data) - MIN_PULSE_DELAY);
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
  
  ENTER_CRITICAL();
  int32_t posCopy = currentPos;
  EXIT_CRITICAL();
  
  STATE state = { 
    mode, 
    (FLAGS)((limitSwitchState == HIGH ? FLAGS::STATE_ON_LIMIT_SWITCH : 0) | 
            (bHomed ? FLAGS::STATE_HOMED : 0)), 
    (uint8_t)iFastPulseDelayMM_Setting, 
    posCopy, 
    targetPos, 
    MIN_POS, 
    MAX_POS 
  };
  Wire.write((uint8_t*)&state, STATE_LEN);
}

// ==================== NON-BLOCKING STATE MACHINE ====================
enum HomingState { HOMING_START, HOMING_SEEKING, HOMING_BACKOFF, HOMING_TO_CENTER };
volatile HomingState homingState = HOMING_START;

void setup() {
  pinMode(stepPin, OUTPUT);
  pinMode(dirPin, OUTPUT);
  pinMode(limiterPinNO, INPUT);
  pinMode(limiterPinNC, INPUT);
  pinMode(LED_PIN, OUTPUT);
  
  digitalWrite(stepPin, LOW);
  digitalWrite(dirPin, LOW);
  
  targetPos = (MIN_POS + MAX_POS) / 2;
  limitSwitchState = digitalRead(limiterPinNO);

  attachInterrupt(limiterPinNO, OnLimitSwitch, CHANGE);

  initStepTimer();
  
  Wire.begin(SLAVE_ADDR);
  Wire.onReceive(receiveEvent);
  Wire.onRequest(requestEvent);

  mode = MODE::UNKNOWN;
  LimitChanged = true;
  currentPos = 0;
}

// ==================== MAIN LOOP ====================
void loop() {
  // Update limit switch state
  if (LimitChanged) {
    LimitChanged = false;
    limitSwitchState = digitalRead(limiterPinNO);
    digitalWrite(LED_PIN, !limitSwitchState);
    
    // If limit switch hit during motion, stop immediately
    if (limitSwitchState == HIGH && steppingEnabled && currentDir == HOME_DIRECTION) {
      stopStepping();
    }
  }

  switch (mode) {
    case MODE::HOMEING:
      handleHoming();
      break;
    case MODE::PARKING:
      handleParking();
      break;
    case MODE::READY:
      handleReady();
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

// ==================== NON-BLOCKING HOMING ====================
void handleHoming() {
  static uint32_t lastBlink = 0;
  
  switch (homingState) {
    case HOMING_START:
      homingState = HOMING_SEEKING;
      currentPos = 0;
      targetPos = RANGE * 1.2;
      // Fall through to seeking
    
    case HOMING_SEEKING:
      // Blink LED during homing
      if (millis() - lastBlink > 250) {
        digitalWrite(LED_PIN, !digitalRead(LED_PIN));
        lastBlink = millis();
      }
      
      // Check if limit reached
      if (limitSwitchState == HIGH) {
        stopStepping();
        homingState = HOMING_BACKOFF;
        
        // Calculate backoff target
        if (HOME_DIRECTION == HIGH) {
          targetPos = currentPos - SAFE_DIST_IN_STEPS;
        } else {
          targetPos = currentPos + SAFE_DIST_IN_STEPS;
        }
        
        setStepFrequency(delayToFreq(HOMEING_PULSE_DELAY));
        startStepping(!HOME_DIRECTION);
      } else if (abs(currentPos) >= RANGE * 1.2) {
        // Switch not found - alarm
        mode = MODE::ALARM;
        bHomed = false;
        stopStepping();
        homingState = HOMING_START;
      } else if (!steppingEnabled) {
        // Continue seeking
        setStepFrequency(delayToFreq(HOMEING_PULSE_DELAY));
        startStepping(HOME_DIRECTION);
      }
      break;
    
    case HOMING_BACKOFF:
      // Wait until backoff complete
      if (currentPos == targetPos) {
        stopStepping();
        limitSwitchState = digitalRead(limiterPinNO);
        currentPos = MAX_POS;
        targetPos = (MIN_POS + MAX_POS) / 2;
        homingState = HOMING_TO_CENTER;
        setStepFrequency(delayToFreq(HOMEING_PULSE_DELAY));
        startStepping((targetPos > currentPos) ? HIGH : LOW);
      }
      break;
    
    case HOMING_TO_CENTER:
      // Wait until center reached
      if (currentPos == targetPos) {
        stopStepping();
        mode = MODE::READY;
        bHomed = true;
        homingState = HOMING_START;
      }
      break;
  }
}

// ==================== NON-BLOCKING PARKING ====================
enum ParkingState { PARKING_START, PARKING_MOVING };
volatile ParkingState parkingState = PARKING_START;

void handleParking() {
  switch (parkingState) {
    case PARKING_START:
      stopStepping();
      targetPos = MIN_POS;
      parkingState = PARKING_MOVING;
      setStepFrequency(delayToFreq(HOMEING_PULSE_DELAY));
      startStepping((targetPos > currentPos) ? HIGH : LOW);
      break;
    
    case PARKING_MOVING:
      if (currentPos == targetPos) {
        stopStepping();
        mode = MODE::READY;
        parkingState = PARKING_START;
      }
      break;
  }
}

// ==================== READY STATE MOTION ====================
void handleReady() {
  if (targetPos != currentPos) {
    // Check if we need to start/restart motion
    if (!steppingEnabled) {
      // Start new motion
      startStepping((targetPos > currentPos) ? HIGH : LOW);
    } else if (currentDir != ((targetPos > currentPos) ? HIGH : LOW)) {
      // Direction changed - restart with new profile
      startStepping((targetPos > currentPos) ? HIGH : LOW);
    }
    // Motion profile is handled by timerISR
  } else {
    // Target reached
    if (steppingEnabled) {
      stopStepping();
    }
  }
}

#endif