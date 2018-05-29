#include <Wire.h>
#include <LiquidCrystal_I2C.h>
LiquidCrystal_I2C lcd(0x27,16,2);

int DELAY_TIME = 20; //Serial

int X = A0; // x
int Y = A1; // y
int S = 8;
int BTN1 = 2; // jump
int BTN2 = 3; // dash

int JOY_STOP = -1;
int JOY_LEFT = 0;
int JOY_RIGHT = 1;

//int JOY_UP = 0;
//int JOY_DOWN = 1;

int BTN_JUMP = 2;
int BTN_DASH = 3;

// 프로그램 시작 - 초기화 작업
void setup()
{
  Serial.begin(9600); // 시리얼 통신 초기화
  
  pinMode(X, INPUT);
  pinMode(Y, INPUT);
  pinMode(S, INPUT);
  digitalWrite(S, HIGH);
  pinMode(S, INPUT);
  
  pinMode(BTN1, INPUT);
  pinMode(BTN2, INPUT);
  
  lcd.begin(); // lcd를 사용을 시작합니다.
  lcd.backlight(); // backlight를 On 시킵니다.
}

void loop()
{
  int x, y, s;
  x = analogRead(X);
  y = analogRead(Y);
  s = digitalRead(S);

//  UP DOWN을 위한 임시코드
//  if(y>=0 && y<=300)
//  {
//    Serial.print(JOY_UP);
//    delay(DELAY_TIME);
//  } else if(y>=800 && y<=1023)
//  {
//    Serial.print(JOY_DOWN);
//    delay(DELAY_TIME);
//  }
  
  if(x>=0 && x<=200)
  {
    Serial.print(JOY_LEFT);
    delay(DELAY_TIME);
  } 
  else if(x>=900 && x<=1023)
  {
    Serial.print(x);
    delay(DELAY_TIME);
  }
  else if(x>=400 && x<=600)
  {
    Serial.print(JOY_STOP);
    delay(DELAY_TIME);
  }
  
  if(digitalRead(BTN1) == HIGH)
  { 
    Serial.print(BTN_JUMP);
    delay(DELAY_TIME);
  }
  if(digitalRead(BTN2) == HIGH)
  {
    Serial.print(BTN_DASH);
    delay(DELAY_TIME);
  }
  
  ReadSerialAndShowLCD();
}

void ReadSerialAndShowLCD()
{
  //한바이트 데이터를 임시 저장
  char cTemp;
  //완성된 명령어
  String sCommand = "";
  String sTemp = "";
  sCommand = "";
  while(Serial.available())
  {
    cTemp = Serial.read();
    sCommand.concat(cTemp);
  }
  //완성된 데이터가 있는지 확인 한다.
  if(sCommand != "" )
  {
    lcd.clear();
    lcd.home(); 
    lcd.print(sCommand);
  }
}
