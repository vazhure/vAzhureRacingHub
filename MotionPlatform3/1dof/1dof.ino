// 3DOF by Andrey Zhuravlev
// e-mail: v.azhure@gmail.com
// version from 2024-02-24

//#define DEBUG
// Board: STM32F103C8T6 4 pcs (master + 3 slave)
// Upload: SWD
#include <Wire.h>  // https://github.com/stm32duino/BoardManagerFiles/raw/main/package_stmicroelectronics_index.json
#include <stdio.h>

// Uncomment this line to flash MASTER device
// Comment this line to flash SLAVE devices
//#define I2CMASTER

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

#ifndef I2CMASTER
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

#define LED_PIN 32

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
                         CMD_SET_ACCEL
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

const int RAW_DATA_LEN = sizeof(PCCMD);

#ifdef I2CMASTER  // MASTER CODE below

#define ALARM_PIN A4
#define SIGNAL_PIN A5

// Device states
STATE st[MAX_LINEAR_ACTUATORS];

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
  Serial.begin(SERIAL_BAUD_RATE);
  delay(1000);
  Wire.begin();

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
  if (Wire.requestFrom(addr, len) == len && Wire.readBytes(ptr, len) == len) {
    return true;
  }
  return false;
}

bool TransmitCMD(uint8_t addr, uint8_t cmd, uint32_t data) {
  Wire.beginTransmission(addr);
  Wire.write(cmd);
  Wire.write((uint8_t*)&data, sizeof(uint32_t));
  return Wire.endTransmission() == 0;
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

const uint32_t dirPin = PA5;
const uint32_t stepPin = PA4;
const uint32_t limiterPinNO = PA6;
const uint8_t limiterPinNC = PA7;

#define ANALOG_INPUT_MAX 4095

uint32_t accel = 900;
const int32_t STEPS_PER_REVOLUTIONS = 1000;                                // Steps per revolution
#define STEPS_CONTROL_DIST STEPS_PER_REVOLUTIONS / 4  // Distance in steps
const int32_t SAFE_DIST_IN_STEPS = STEPS_PER_REVOLUTIONS / 2;              // Safe traveling distance in steps
const float MM_PER_REV = 5.0f;                                             // distance in mm per revolution
const float MAX_REVOLUTIONS = 18;                                          // maximum revolutions
const int32_t RANGE = (int32_t)(MAX_REVOLUTIONS * STEPS_PER_REVOLUTIONS);  // Maximum traveling distance, steps
const int32_t MIN_POS = SAFE_DIST_IN_STEPS;                                // minimal controlled position, steps
const int32_t MAX_POS = RANGE - SAFE_DIST_IN_STEPS;                        // maximal controlled position, steps
const uint8_t HOME_DIRECTION = HIGH;

#define MIN_PULSE_DELAY 10  // Minimal pulse interval, us

#define MIN_REVERSE_DELAY 6  // Delay between DIR and STEP signal on direction change, us

#define MMPERSEC2DELAY(mmps) 1000000 / (STEPS_PER_REVOLUTIONS * mmps / MM_PER_REV)

#define MAX_SPEED_MM_SEC 150
#define MIN_SPEED_MM_SEC 10
#define SLOW_SPEED_MM_SEC 20
#define DEFAULT_SPEED_MM_SEC 90

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

inline void Step(uint8_t dir, int delay) {
  if (limitSwitchState == HIGH && dir == HOME_DIRECTION)
    return;  // ON LIMIT SWITCH

  digitalWrite(dirPin, dir);
  if (_oldDir != dir) {
    last_step_time = 0;
    delayMicroseconds(MIN_REVERSE_DELAY);
    _oldDir = dir;
  }

  float delta = MIN((micros() - last_step_time), iSlowPulseDelay);
  uint32_t pulse = MAX(delay, ceil(delta - (float)accel / delta));

  digitalWrite(stepPin, HIGH);
  delayMicroseconds(MIN_PULSE_DELAY);
  digitalWrite(stepPin, LOW);
  delayMicroseconds(pulse);
  currentPos += dir == HIGH ? 1 : -1;
  last_step_time = micros();
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
        if (mode != MODE::HOMEING;) {
          mode = MODE::HOMEING;
          currentPos = 0;
          targetPos = MAX_POS;
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
          targetPos = data;
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
        iFastPulseDelayMM = data = std::clamp((int)data, MIN_SPEED_MM_SEC, MAX_SPEED_MM_SEC);
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

        if (abs(currentPos) >= RANGE)  // SWITCH NOT FOUND
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