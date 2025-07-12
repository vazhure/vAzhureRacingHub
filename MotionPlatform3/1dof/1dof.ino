// 3DOF by Andrey Zhuravlev
// e-mail: v.azhure@gmail.com
// version from 2025-06-21
// stm32duino version

// NOTE: select fake STM32F103C8
// https://www.stm32duino.com/

//#define INVERTED_DIR

//#define DEBUG
// Board: STM32F103C8T6 5 pcs (master + 4 slave)
// Upload: SWD
#define I2C_SDA PB7
#define I2C_SCL PB6
#include <Wire_slave.h>
#include <stdio.h>

// Uncomment this line if you using SF1610
#define SFU1610 // Used only in SLAVE devices

// Uncomment this line to flash MASTER device
// Comment this line to flash SLAVE devices
#define I2CMASTER

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
#define SLAVE_ADDR SLAVE_ADDR_FL
//#define SLAVE_ADDR SLAVE_ADDR_RL
//#define SLAVE_ADDR SLAVE_ADDR_RR
//#define SLAVE_ADDR SLAVE_ADDR_FR
#endif

#define SERIAL_BAUD_RATE 115200

// override Serial Port buffer size
#define SERIAL_TX_BUFFER_SIZE 512
#define SERIAL_RX_BUFFER_SIZE 512

#define LED_PIN PC13 // * Check your board. Some have PB2 or another (label near LED).

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
  uint8_t speedMMperSEC;
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

// Alarm input event
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
  // scanning slave devices
  for (int addr = SLAVE_FIRST; addr <= SLAVE_LAST; addr++) {
    Wire.beginTransmission(addr);
    uint8_t err = Wire.endTransmission();
    SET_SLAVE_STATE(addr - SLAVE_FIRST, err == 0);
    if (err == 0) nFound++;
  }
  digitalWrite(LED_PIN, nFound == 0 ? HIGH : LOW);

  Serial.begin(SERIAL_BAUD_RATE);
  while (!Serial)
    ;  // whait connected

#ifdef DEBUG
#define RETRY_CNT 3
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
  if (bAlarm) {
    for (int addr = SLAVE_FIRST; addr <= SLAVE_LAST; addr++)
      TransmitCMD(addr, COMMAND::SET_ALARM, 1);
    bAlarm = false;
  }

  if (Serial.available())
    serialEvent();

  if (_bDataPresent) {
    digitalWrite(LED_PIN, LOW);  // Turn LED on
    switch (pccmd.cmd) {
      case COMMAND::CMD_HOME:
      case COMMAND::CMD_ENABLE:
      case COMMAND::CMD_DISABLE:
      case COMMAND::CMD_CLEAR_ALARM:
        {
          for (int t = 0; t < LINEAR_ACTUATORS; t++) {
            if (pccmd.data[t] == 1)
              TransmitCMD(SLAVE_FIRST + t, pccmd.cmd, 0);
          }
        }
        break;
      case COMMAND::CMD_MOVE_SH:
        {
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

uint32_t accel = 10000;
#define STEPS_CONTROL_DIST STEPS_PER_REVOLUTIONS / 4  // Distance in steps

#ifdef SFU1610
const float MM_PER_REV = 10.0f;                                // distance in mm per revolution
const float MAX_REVOLUTIONS = 8.75;                            // maximum revolutions
const int32_t STEPS_PER_REVOLUTIONS = 2000;                    // Steps per revolution
const int32_t SAFE_DIST_IN_STEPS = STEPS_PER_REVOLUTIONS / 4;  // Safe traveling distance in steps
#define MAX_SPEED_MM_SEC 240  // maximum speed mm/sec
#else
const float MM_PER_REV = 5.0f;                                 // distance in mm per revolution
const float MAX_REVOLUTIONS = 17.5;                            // maximum revolutions
const int32_t STEPS_PER_REVOLUTIONS = 1000;                    // Steps per revolution
const int32_t SAFE_DIST_IN_STEPS = STEPS_PER_REVOLUTIONS / 2;  // Safe traveling distance in steps
#define MAX_SPEED_MM_SEC 120  // maximum speed mm/sec
#endif

const int32_t RANGE = (int32_t)(MAX_REVOLUTIONS * STEPS_PER_REVOLUTIONS);  // Maximum traveling distance, steps
const int32_t MIN_POS = SAFE_DIST_IN_STEPS;                                // minimal controlled position, steps
const int32_t MAX_POS = RANGE - SAFE_DIST_IN_STEPS;                        // maximal controlled position, steps
const uint8_t HOME_DIRECTION = HIGH;

#define MIN_PULSE_DELAY 10  // Minimal pulse interval, us

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
uint8_t limitSwitchState = HIGH;
volatile bool bHomed = false;
int32_t currentPos = 0;
uint8_t _oldDir = LOW;  // previous DIR state

volatile bool LimitChanged = true;
uint32_t last_step_time = 0;
double pulse = iSlowPulseDelay;

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

// Limit switch event
void OnLimitSwitch() {
  LimitChanged = true;
}

void receiveEvent(int size) {
  uint8_t cmd;
  uint32_t data;
  if (size >= 5 && Wire.readBytes(&cmd, 1) == 1 && Wire.readBytes((uint8_t*)&data, sizeof(uint32_t)) == sizeof(uint32_t)) {
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
        if (mode == MODE::READY)
          targetPos = clamp(data, (uint32_t)MIN_POS, (uint32_t)MAX_POS);
        break;
      case COMMAND::CMD_CLEAR_ALARM:
      case COMMAND::CMD_ENABLE:
        {
          LimitChanged = true;
          if (bHomed)
            mode = MODE::READY;
          else
            mode = MODE::CONNECTED;
        }
        break;
      case COMMAND::CMD_DISABLE:
        mode = MODE::DISABLED;
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
        break;
    }
  }
}

void requestEvent() {
  if (mode == MODE::UNKNOWN)
    mode = MODE::CONNECTED;
  STATE state = { mode, (FLAGS)((limiterPinNO == HIGH ? FLAGS::STATE_ON_LIMIT_SWITCH : 0) | (bHomed ? FLAGS::STATE_HOMED : 0)), (uint8_t)iFastPulseDelayMM, currentPos, targetPos, MIN_POS, MAX_POS };
  Wire.write((uint8_t*)&state, STATE_LEN);
}

// Initializtion
void setup() {
  pinMode(stepPin, OUTPUT);
  pinMode(dirPin, OUTPUT);
  pinMode(limiterPinNO, INPUT);
  pinMode(limiterPinNC, INPUT);
  pinMode(LED_PIN, OUTPUT);
  targetPos = (MIN_POS + MAX_POS) / 2;

  limitSwitchState = digitalRead(limiterPinNO);

  attachInterrupt(limiterPinNO, OnLimitSwitch, CHANGE);

  Wire.begin(SLAVE_ADDR);
  Wire.onReceive(receiveEvent);
  Wire.onRequest(requestEvent);

  mode = MODE::UNKNOWN;
  LimitChanged = true;
  currentPos = 0;
}

int inc = -1;

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
        if (limitSwitchState == HIGH)  // SITTING ON SWITH
        {
          for (int32_t t = 0; t < SAFE_DIST_IN_STEPS; t++) {
            Step(!HOME_DIRECTION, HOMEING_PULSE_DELAY);
          }

          currentPos = MAX_POS;
          targetPos = (MIN_POS + MAX_POS) / 2;
          // Moving to the center postion at homing speed
          while (targetPos != currentPos)
            Step(targetPos > currentPos ? HIGH : LOW, HOMEING_PULSE_DELAY);

          mode = MODE::READY;
          bHomed = true;

          break;
        }

        digitalWrite(LED_PIN, (millis() % 500) > 250 ? HIGH : LOW);

        if (abs(currentPos) >= RANGE * 1.2)  // SWITCH NOT FOUND
        {
          mode = MODE::ALARM;
          bHomed = false;
          break;
        }
        Step(HOME_DIRECTION, HOMEING_PULSE_DELAY);
      }
      break;
    case MODE::PARKING:
      {
        targetPos = MIN_POS;
        while (targetPos != currentPos)
          Step(targetPos > currentPos ? HIGH : LOW, HOMEING_PULSE_DELAY);
        mode = MODE::READY;
      }
      break;
    case MODE::READY:
      {
        if (targetPos != currentPos) {
          long dist = constrain(abs(targetPos - currentPos), 0, STEPS_CONTROL_DIST);
          int delay = map(dist, 0, STEPS_CONTROL_DIST, iSlowPulseDelay, iFastPulseDelay);
          Step(targetPos > currentPos ? HIGH : LOW, delay);
        }
      }
      break;
    case MODE::ALARM:
      digitalWrite(LED_PIN, (millis() % 250) > 125 ? HIGH : LOW);
      break;
    default: break;
  }
}
#endif
