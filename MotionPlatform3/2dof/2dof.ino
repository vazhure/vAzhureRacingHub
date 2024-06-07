// 2DOF by Andrey Zhuravlev
// e-mail: v.azhure@gmail.com
// version from 2024-06-07

//#define DEBUG
#include <stdio.h>

#define ROT_POT1_PIN A0  // MOTOR A POSITION [0..1024]
#define ROT_POT2_PIN A1  // MOTOR B POSITION [0..1024]

#define M1_PWM_PIN 5
#define M1_DIR_PIN 7
#define M2_PWM_PIN 6
#define M2_DIR_PIN 8

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

struct __attribute__((packed)) STATE {
  MODE mode;
  FLAGS flags;
  uint8_t speedMMperSEC;  // Max PWM in percents [10.100]
  uint8_t _reserved;
  int32_t currentpos;  // motor pos
  int32_t targetpos;   // motor target pos
  int32_t min;         // min pos (in poteciometer units)
  int32_t max;         // max pos (in poteciometer units)
};

const int STATE_LEN = sizeof(STATE);  // should be 20

struct __attribute__((packed)) PCCMD {
  uint8_t header = 0;
  uint8_t len;  // len
  COMMAND cmd;
  uint8_t reserved;
  int32_t data[MAX_LINEAR_ACTUATORS];
} pccmd;

const int RAW_DATA_LEN = sizeof(PCCMD);  // should be 20

// Device states
STATE st[MAX_LINEAR_ACTUATORS];

#define POS_MIN 100  // MODIFY TO WORKING RANGE
#define POS_MAX 900  // MODIFY TO WORKING RANGE
#define POS_CENTER (POS_MIN + POS_MAX) / 2

void setup() {
  // Bigger PWM frequency - less motor noise
  setPwmFrequency(M1_PWM_PIN, 8);  // About 7.8 kHz
  setPwmFrequency(M2_PWM_PIN, 8);  // About 7.8 kHz

  // read actual position
  int32_t posA = analogRead(ROT_POT1_PIN);
  int32_t posB = analogRead(ROT_POT2_PIN);

  // init structures
  st[0] = { MODE::UNKNOWN /*set MODE::READY for 3DOF*/, FLAGS::STATE_HOMED, 100, 0, POS_CENTER, POS_CENTER, POS_MIN, POS_MAX };  // NOT CONNECTED
  st[1] = { MODE::READY, FLAGS::STATE_HOMED, 100, posA, 0, POS_CENTER, POS_MIN, POS_MAX };
  st[2] = { MODE::READY, FLAGS::STATE_HOMED, 100, posB, 0, POS_CENTER, POS_MIN, POS_MAX };
  st[3] = { MODE::UNKNOWN, FLAGS::STATE_HOMED, 100, 0, POS_CENTER, POS_CENTER, POS_MIN, POS_MAX };  // NOT CONNECTED

  Serial.begin(SERIAL_BAUD_RATE);
}

#define CMD_ID 0
// serial input buffer
uint8_t buf[RAW_DATA_LEN * 2];
int offset = 0;
volatile bool _bDataPresent = false;
unsigned long _lasttime;
uint32_t accel = 900;
uint32_t speed = 100;

void ProcessCMD() {
  switch (pccmd.cmd) {
    case COMMAND::CMD_ENABLE:
    case COMMAND::CMD_DISABLE:
    case COMMAND::CMD_CLEAR_ALARM:
      {
        // TODO:
      }
      break;
    case COMMAND::CMD_MOVE:
      {
        for (int t = 0; t < LINEAR_ACTUATORS; t++)
          st[t].targetpos = pccmd.data[t];
      }
      break;
    case COMMAND::CMD_HOME:
    case COMMAND::CMD_PARK:
      {
        for (int t = 0; t < LINEAR_ACTUATORS; t++)
          st[t].targetpos = POS_CENTER;
      }
      break;
    case COMMAND::CMD_SET_SPEED:
      {
        speed = min(100, pccmd.data[1]);
      }
      break;
    case COMMAND::CMD_SET_SLOW_SPEED:
      {
        // TODO:
      }
      break;
    case COMMAND::CMD_SET_ACCEL:
      {
        accel = pccmd.data[1];
      }
      break;
    case COMMAND::CMD_GET_STATE:
      {
        for (int t = 0; t < LINEAR_ACTUATORS; t++) {
          Serial.write(SLAVE_FIRST + t);
          Serial.write(STATE_LEN);
          Serial.write((uint8_t*)&st[t], STATE_LEN);
        }
      }
      break;
  }
}

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
        ProcessCMD();
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

// Setup PWM clock divider
void setPwmFrequency(int pin, int divisor) {
  byte mode;
  if (pin == 5 || pin == 6 || pin == 9 || pin == 10) {
    switch (divisor) {
      case 1:
        mode = 0x01;
        break;  // 62.5 kHz
      case 8:
        mode = 0x02;
        break;  // 7.8 kHz
      case 64:
        mode = 0x03;
        break;  // 976 Hz
      case 256:
        mode = 0x04;
        break;  // 244 Hz
      case 1024:
        mode = 0x05;
        break;  // 61 Hz
      default:
        return;
    }
    if (pin == 5 || pin == 6) {
      TCCR0B = TCCR0B & 0b11111000 | mode;
    } else {
      TCCR1B = TCCR1B & 0b11111000 | mode;
    }
  } else if (pin == 3 || pin == 11) {
    switch (divisor) {
      case 1:
        mode = 0x01;
        break;  // 31.25 kHz
      case 8:
        mode = 0x02;
        break;  // 3.9 kHz
      case 32:
        mode = 0x03;
        break;  // 976 Hz
      case 64:
        mode = 0x04;
        break;  // 488 Hz
      case 128:
        mode = 0x05;
        break;  // 244 Hz
      case 256:
        mode = 0x06;
        break;  // 122 Hz
      case 1024:
        mode = 0x07;
        break;  // 30 Hz
      default:
        return;
    }
    TCCR2B = TCCR2B & 0b11111000 | mode;
  }
}

// previous values
float _pwm1 = 0.f;
float _pwm2 = 0.f;

void loop() {
  st[1].currentpos = analogRead(ROT_POT1_PIN);
  st[2].currentpos = analogRead(ROT_POT2_PIN);

  // Motors control code

  if (st[1].currentpos != st[1].targetpos) {
    // TODO: MOVE MOTOR A
  } else {
    // STOP MOTOR A
  }

  if (st[2].currentpos != st[1].targetpos) {
    // TODO: MOVE MOTOR B
  } else {
    // STOP MOTOR B
  }
}