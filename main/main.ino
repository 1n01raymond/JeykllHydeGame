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
    Serial.print(',');
    delay(DELAY_TIME);
  } 
  else if(x>=900 && x<=1023)
  {
    Serial.print(JOY_RIGHT);
    Serial.print(',');
    delay(DELAY_TIME);
  }
  else if(x>=400 && x<=600)
  {
    delay(DELAY_TIME);
  }
  
  if(digitalRead(BTN1) == HIGH)
  { 
    Serial.print(BTN_JUMP);
    Serial.print(',');
    delay(DELAY_TIME);
  }
  if(digitalRead(BTN2) == HIGH)
  {
    Serial.print(BTN_DASH);
    Serial.print(',');
    delay(DELAY_TIME);
  }
}

