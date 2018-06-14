#include <Wire.h>
#include <LiquidCrystal_I2C.h>
LiquidCrystal_I2C lcd(0x27, 16, 2);

byte gaugeNone[8] = {
  B00000,
  B00000,
  B00000,
  B00000,
  B00000,
  B00000,
  B00000,
  B00000
};
byte gaugeFull[8] = {
  B11111,
  B11111,
  B11111,
  B11111,
  B11111,
  B11111,
  B11111,
  B11111
};

// set the LCD address to 0x27 for a 16 chars and 2 line display
void setup()
{
  // initialize the LCD
  Serial.begin(9600); // 시리얼 통신 초기화
  lcd.begin(); // lcd를 사용을 시작합니다.
  lcd.backlight(); // backlight를 On 시킵니다.
  lcd.print("HAPPINESS"); // 화면에 Hello, world!를 출력합니다.   
  lcd.createChar(0, gaugeNone);
  lcd.createChar(1, gaugeFull);
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
    happiness = sCommand.toInt();
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
  }
}
