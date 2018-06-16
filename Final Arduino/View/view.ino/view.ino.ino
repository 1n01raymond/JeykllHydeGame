#include <Wire.h>
#include <LiquidCrystal_I2C.h>
LiquidCrystal_I2C lcd(0x27, 16, 2);

// RAW/COL Arduino port defines
#define ROW_1 2
#define ROW_2 3
#define ROW_3 4
#define ROW_4 5
#define ROW_5 6
#define ROW_6 7
#define ROW_7 8
#define ROW_8 9

#define COL_1 10
#define COL_2 11
#define COL_3 12
#define COL_4 13
#define COL_5 A0
#define COL_6 A1
#define COL_7 A2
#define COL_8 A3

const byte rows[] = {
  ROW_1, ROW_2, ROW_3, ROW_4, ROW_5, ROW_6, ROW_7, ROW_8
};

byte emotions[2][8] = {
  {B00000000, B11000110, B11000110, B00010000, B00010000, B00000000, B01111100, B10000010}, // SAD
  {B00000000, B11000110, B11000110, B00010000, B00010000, B00000000, B10000010, B01111100}  // SMILE
};

byte LCDGaugeNone[8] = {
  B00000,
  B00000,
  B00000,
  B00000,
  B00000,
  B00000,
  B00000,
  B00000
};
byte LCDGaugeFull[8] = {
  B11111,
  B11111,
  B11111,
  B11111,
  B11111,
  B11111,
  B11111,
  B11111
};

float timeCount = 0;

void setup()
{
  // Open serial port
  Serial.begin(9600);

  lcd.begin(); // lcd를 사용을 시작합니다.
  lcd.backlight(); // backlight를 On 시킵니다.
  lcd.print("HAPPINESS"); // 화면에 Hello, world!를 출력합니다.   
  lcd.createChar(0, LCDGaugeNone);
  lcd.createChar(1, LCDGaugeFull);

  for (byte i = 2; i <= 13; i++)
  {
    pinMode(i, OUTPUT);
  }
  pinMode(A0, OUTPUT);
  pinMode(A1, OUTPUT);
  pinMode(A2, OUTPUT);
  pinMode(A3, OUTPUT);
}

void loop()
{
  //한바이트 데이터를 임시 저장
  char cTemp;
  //완성된 명령어
  String sCommand = "";
  String sTemp = "";
  int happiness = 0;
  sCommand = "";
  while (Serial.available())
  {
    cTemp = Serial.read();
    sCommand.concat(cTemp);
  }
  //완성된 데이터가 있는지 확인 한다.
  if (sCommand != "")
  {
    //LCD
    happiness = sCommand.toInt();
    Serial.println(happiness);
    lcd.setCursor(0, 1); // Set lcd cursor to the start of the first row
    int i = 0;
    while (i<16)
    {
      if (i>=(happiness/6.25))
      {
        lcd.write(0);
      }
      else {
        lcd.write(1);
      }
      i++;
    }

    if ( happiness < 1 )
    {
      return ;
    }
    
   //metrix
    int emotion = (happiness>=50)?1:0;
    
    for ( int i = 0; i < 200; i++ ) // 200 : how long we will show the emotion
    {
      drawScreen(emotions[emotion]);
    }
  }
}

void  drawScreen(byte buffer2[])
{
  // Turn on each row in series
  for (byte i = 0; i < 8; i++)
  {
    setColumns(buffer2[i]); // Set columns for this specific row

    digitalWrite(rows[i], HIGH);
    delay(2); // Set this to 50 or 100 if you want to see the multiplexing effect!
    digitalWrite(rows[i], LOW);
  }
}

void setColumns(byte b)
{
  digitalWrite(COL_1, (~b >> 0) & 0x01); // Get the 1st bit: 10000000
  digitalWrite(COL_2, (~b >> 1) & 0x01); // Get the 2nd bit: 01000000
  digitalWrite(COL_3, (~b >> 2) & 0x01); // Get the 3rd bit: 00100000
  digitalWrite(COL_4, (~b >> 3) & 0x01); // Get the 4th bit: 00010000
  digitalWrite(COL_5, (~b >> 4) & 0x01); // Get the 5th bit: 00001000
  digitalWrite(COL_6, (~b >> 5) & 0x01); // Get the 6th bit: 00000100
  digitalWrite(COL_7, (~b >> 6) & 0x01); // Get the 7th bit: 00000010
  digitalWrite(COL_8, (~b >> 7) & 0x01); // Get the 8th bit: 00000001

  // If the polarity of your matrix is the opposite of mine
  // remove all the '~' above.
}
