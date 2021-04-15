#include <SPI.h>
#include <Servo.h>

// Pins setup
const int nReset = 1;
const int nCS = 2;
const int nPD = 3;
const int trigger_pin = 0;

const int out_A = 4;
const int out_B = 5;
const int out_C = 6;

// Global variables
unsigned long ElapsedTime = 0;
unsigned long StartTime = 0;
boolean state = false;
String s_out;
byte incomingByte = 0; 

Servo servo1;

void setup() {
  //Configure pins
  pinMode (nReset, OUTPUT);
  pinMode (nCS, OUTPUT);
  pinMode (nPD, OUTPUT);
  pinMode (trigger_pin, INPUT_PULLUP);
  pinMode (out_A, OUTPUT);
  pinMode (out_B, OUTPUT);
  pinMode (out_C, OUTPUT);
  attachInterrupt(digitalPinToInterrupt(trigger_pin), trigger, CHANGE);

  pinMode(A3,OUTPUT);
  servo1.attach(A3);

  //Configure initial state (select chip)
  digitalWrite(nReset, LOW);
  digitalWrite(nCS, HIGH);
  digitalWrite(nPD, HIGH);
  
  //Initialize SPI
  SPI.begin();
  SPI.beginTransaction(SPISettings(2000000, MSBFIRST, SPI_MODE3));

  //Start a serial port
  Serial.begin(115200);

  //Wait for chip boot
  delay(100);

}

void loop() {
  if (Serial.available() > 0) {
    incomingByte = Serial.read();
    switch (incomingByte) {
      
      case 65:
        //Serial.println("A on");
        digitalWrite(out_A, HIGH);
        break;
      case 66:
        //Serial.println("B on");
        digitalWrite(out_B, HIGH);
        break;
      case 67:
        //Serial.println("C on");
        digitalWrite(out_C, HIGH);
        break;
        
      case 97:
        //Serial.println("A off");
        digitalWrite(out_A, LOW);
        break;
      case 98:
        //Serial.println("B off");
        digitalWrite(out_B, LOW);
        break;
      case 99:
        //Serial.println("C off");
        digitalWrite(out_C, LOW);
        break;

      case 82:
        //Reset
        StartTime = millis();
        break;

      case 48:
        servo1.writeMicroseconds(800);
        break;
      case 49:
        servo1.writeMicroseconds(900);
        break;
      case 50:
        servo1.writeMicroseconds(1000);
        break;
      case 51:
        servo1.writeMicroseconds(1100);
        break;
      case 52:
        servo1.writeMicroseconds(1200);
        break;
      case 53:
        servo1.writeMicroseconds(1300);
        break;
         
      default: 
        //Serial.println("Invalid command");
        break;
    }
  }
  
  //Wait x miliseconds for data to collect
  delay(3);

  //Read motion
  signed char motion = ADNS_read(0x02);
  //char xdiff = ADNS_read(0x03);
  signed char ydiff = ADNS_read(0x04);

  s_out = String(millis() - StartTime) + " " + String(ydiff,DEC) + " " + String(state);
  Serial.println(s_out);
  //Serial.println(String((int)ADNS_read(0x04),DEC));
}

void ADNS_write(unsigned int address, unsigned int data) {
  // Take the CS pin low to select the chip:
  digitalWrite(nCS, LOW);

  // Send in the address and value via SPI:
  SPI.transfer(address);
  SPI.transfer(data);

  // Take the SS pin high to de-select the chip:
  digitalWrite(nCS, HIGH);
}

int ADNS_read(unsigned int address) {
  int data;

  // Take the CS pin low to select the chip:
  digitalWrite(nCS, LOW);
  
  // Send in the address and value via SPI:
  SPI.transfer(address);
  
  // Wait for the chip
  delayMicroseconds(50);
  
  // Read out the requested value
  data = (signed char)SPI.transfer(0x00);
  
  // Take the SS pin high to de-select the chip:
  digitalWrite(nCS, HIGH);
  
  return (data);
}

void trigger() {
  state = !state;
  StartTime = millis();
}


