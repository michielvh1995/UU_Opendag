unsigned long startSpray = 0;
unsigned long currentMillis = 0;
unsigned int sprayTime = 30000;
int sprayPin = 13;


bool spray=0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  Serial.print("Hey");
  pinMode(sprayPin,OUTPUT);
  pinMode(13,OUTPUT);
}

void loop() {
  // put your main code here, to run repeatedly:
  currentMillis = millis();
  
  if (Serial.available() > 0) {
    byte incomingByte = Serial.read();
    
    Serial.print("Input");
      
    // Set the thing to spray:
    spray = true;
    startSpray = currentMillis;
    Serial.flush();
  }
  
  if(currentMillis - startSpray > sprayTime && spray){
    spray = false;
  }
  Serial.print(spray);
  digitalWrite(sprayPin,spray);
  digitalWrite(13,spray);
}
