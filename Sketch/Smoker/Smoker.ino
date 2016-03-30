// sketch_12_04_server_wifi

#include <SPI.h>
#include <WiFi101.h>

char ssid[] = "BeeBar2";  // your network SSID (name)
char pass[] = "9254582716";     // your network password

WiFiServer server(80);
WiFiClient client;

const int numPins = 10;
int pins[] = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
int pinState[] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
char line1[100];
char buffer[100];

IPAddress server_IP(192,168,123,90) ;

int reading = 0;
float targetTemperature = 225;
float smokerTemperature = 225;
float v0;

void setup()
{
  Serial.begin(9600);
  while (!Serial) {}; // Leonardo needs this

  for (int i = 0; i < numPins; i++)
  {
    pinMode(pins[i], OUTPUT);
  }
  // change the ADC resolution to 12 bits
  analogReadResolution(12);

  if (WiFi.begin(ssid, pass))
  {
    WiFi.config(server_IP);
    printWifiStatus();
    //Serial.print("Point your Browser at: http://");
    //Serial.println(WiFi.localIP());
  }
  else
  {
    Serial.println("Could not connect to network");
  }
  server.begin();
}

void loop()
{
  client = server.available();
  if (client)
  {
    if (client.connected())
    {
      Serial.print("Request Received: ");
      Serial.println(line1);

      readHeader();
      if (! pageNameIs("/"))
      {
        Serial.print("Invalid Page Requested: ");
        Serial.println(line1);
        client.stop();
        return;
      }
      readTargetTempParam();
      client.println("HTTP/1.1 200 OK\nContent-Type: text/html\n");
      sendBody();
      delay(1);
      client.stop();
    }
    else
    {
      Serial.println("No Client connected ");
    }
  }
  reading = analogRead(0);
  v0 = (float) reading / 1241.0;
  smokerTemperature = ThermisterTemp(v0, 100000, 10000, 3950, 25);
  digitalWrite(0, smokerTemperature<targetTemperature);
}

void sendBody()
{
  client.println("<html><body>");
  sendAnalogReadings();
  client.println("<form method='GET'>");
  client.print("\n<h1>Target Temperature:</h1> <input type='number' value='");
  client.print(targetTemperature);
  client.println("' name='t'><br>");
  //client.println("<h1>Output Pins</h1>\);
  //setValuesFromParams();
  //setPinStates();
  //sendHTMLforPins();
  client.println("<input type='submit' value='Update'/>\n</form>\n</body></html>");
}

void sendAnalogReadings()
{
  client.println("<h1>Temperatures</h1>\n<table border='1'>");
  
  Serial.print("Pin(");Serial.print(0);Serial.print("): ");Serial.print(reading);Serial.print(" V=");Serial.print(v0);Serial.print(" Temperature="); Serial.println(smokerTemperature);
 
  client.print("<tr><td>Smoker ");
  client.print("</td><td>"); client.print(smokerTemperature,1);
  client.println(" F</td></tr>");
    
  for (int i = 1; i < 7; i++)
  {
    int reading = analogRead(i);
    if (reading > 200) // only send ungrounded inputs
    {
      float v = (float) reading / 1241.0;
      float T = ThermisterTemp(v, 100000, 10000, 3950, 25);
      Serial.print("Pin(");Serial.print(i);Serial.print("): ");Serial.print(reading);Serial.print(" V=");Serial.print(v);Serial.print(" Temperature="); Serial.println(T);
      client.print("<tr><td>Probe "); client.print(i);
      client.print("</td><td>"); client.print(T,1);
      client.println(" F</td></tr>");
    }
  }
  client.println("</table>");
}

void sendHTMLforPins()
{
  for (int i = 0; i < numPins; i++)
  {
    client.print("<p>Pin ");
    client.print(pins[i]);
    client.print("<select name='");
    client.print(i);
    client.println("'>\n<option value='0'");
    if (pinState[i] == 0)
    {
      client.print(" selected");
    }
    client.println(">Off</option>\n<option value='1'");
    if (pinState[i] == 1)
    {
      client.print(" selected");
    }
    client.println(">On</option>\n</select></p>");
  }
}

void setPinStates()
{
  for (int i = 0; i < numPins; i++)
  {
    digitalWrite(pins[i], pinState[i]);
  }
}

void setValuesFromParams()
{
  for (int i = 0; i < numPins; i++)
  {
    pinState[i] = valueOfParam(i + '0');
  }
}

void readHeader()
{
  readRequestLine(line1);
  while (readRequestLine(buffer) > 1 && buffer[0] != '\n') {}
}

int readRequestLine(char *line)
{
  char ch;
  int i = 0;
  while (ch != '\n' && i < 100 && client.available())
  {
    if (client.available())
    {
      ch = client.read();
      line[i] = ch;
      i ++;
    }
  }
  line[i] = '\0';
  return i;
}

boolean pageNameIs(char* name)
{
  // page name starts at char pos 4
  // ends with space
  int i = 4;
  char ch = line1[i];
  while (ch != ' ' && ch != '\n' && ch != '?')
  {
    if (name[i - 4] != line1[i])
    {
      return false;
    }
    i++;
    ch = line1[i];
  }
  return true;
}

float readTargetTempParam()
{
  //Serial.print("parsing targetTemperature");Serial.println(line1);
  
  int len = strlen(line1);
  for (int i = 0; i < len; i++)
  {
    if (line1[i] == 't' && line1[i + 1] == '=')
    {
      float val = 0;
      for (int j=i+2; j< len; j++)
      {
        if (isDigit(line1[j]))
        {
          //Serial.print(line1[j]);
          val *= 10;
          val += (int)line1[j]-'0';
        }
        else
        {
          break;
        }
      }
      //Serial.print(" ");
      targetTemperature = val;
      //Serial.print("targetTemperature = ");Serial.println(val);
      return val;
    }
  }
  return -1;
}

int valueOfParam(char param)
{
  for (int i = 0; i < strlen(line1); i++)
  {
    if (line1[i] == param && line1[i + 1] == '=')
    {
      return (line1[i + 2] - '0');
    }
  }
  return 0;
}

void printWifiStatus() {
  // print the SSID of the network you're attached to:
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());

  // print your WiFi shield's IP address:
  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);

  // print the received signal strength:
  long rssi = WiFi.RSSI();
  Serial.print("signal strength (RSSI):");
  Serial.print(rssi);
  Serial.println(" dBm");
}

float ThermisterTemp(float Vtherm, int R0therm, int Rfixed, int Beta, int T0)
{
  // Current throught the 2 rsiteres is the same, so compute the current 
  // throught the fixed resiter using Ohms law I = V / R
  // The voltage drop over the fixed resister is Vcc (3.3V) - Vtherm
  double I = (3.3 - Vtherm) / Rfixed;
  //Serial.print("I=");Serial.print(I,12);Serial.print(" ");
  
  // Use ohms law to cacultate the resitance of the thermister R = V / I
  double Rtherm = Vtherm / I;
  //Serial.print("Rtherm=");Serial.print(Rtherm);Serial.print(" ");

  // Use the Beta eaquation to calulate the tempuratue in C
  // 1/T = (1/Beta)*ln(Rtherm/R0therm) + (1/T0)
  // (Steinhart-Hart equatin is more accutrate, but I only have the Beta constant 
  // for my thermister.

  // All temperatres are in Kelvin
  T0 += 273.15;
  
  //double invBeta = ((double) 1)/((double) Beta);
  //Serial.print("InvBeta=");Serial.print(invBeta,12);Serial.print(" ");
  //double lnRoverR0 =log((Rtherm)/((double)R0therm));
  //Serial.print("lnRoverR0=");Serial.print(lnRoverR0,12);Serial.print(" ");
  //double invT0 = ((double)1)/((double)T0);
  //Serial.print("invT0=");Serial.print(invT0,12);Serial.print(" ");
  //double invT = invBeta*lnRoverR0 + invT0;
  //Serial.print("invT=");Serial.print(invT,12);Serial.print(" ");
  //double T = ((double)1.0)/invT;
  
  double T = 1.0/( (1.0/((double)Beta))*log((Rtherm)/((double)R0therm)) + (1.0/((double)T0)) );
  //Serial.print("T=");Serial.print(T);Serial.print(" ");

  //Serial.println("");
  // convert to farenheit for us ignorant Americans
  return (float) ((9*(T-273.15))/5)+32;
}





