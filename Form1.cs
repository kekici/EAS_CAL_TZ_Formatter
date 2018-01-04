using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAS_CAL_TZ_Formatter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ConvertBtn_Click(object sender, EventArgs e)
        {


            // A sample TimeZone info from trace
            //< TimeZone > iP///1QAdQByAGsAZQB5ACAAUwB0AGEAbgBkAGEAcgBkACAAVABpAG0AZQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEABQABAAAAAAAAAAAAAAAAACgAVQBUAEMAKwAwADMAOgAwADAAKQAgAEkAcwB0AGEAbgBiAHUAbAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMAAAAFAAMAAAAAAAAAxP///w==</TimeZone>
            // content is not readable .. used below documentation and created this program for conversion.
            // kekici@microsoft.com
            // http://interoperability.blob.core.windows.net/files/MS-ASDTYPE/[MS-ASDTYPE].pdf
            //Bias (4 bytes): The value of this field is a LONG, as specified in [MS-DTYP] section 2.2.27. The offset from UTC, in minutes. For example, the bias for Pacific Time (UTC-8) is 480. 
            //StandardName(64 bytes): The value of this field is an array of 32 WCHARs, as specified in [MSDTYP] section 2.2.60. It contains an optional description for standard time.Any unused WCHARs in the array MUST be set to 0x0000. 
            //StandardDate (16 bytes): The value of this field is a SYSTEMTIME structure, as specified in [MSDTYP] section 2.3.13. It contains the date and time when the transition from DST to standard time occurs.
            //StandardBias (4 bytes): The value of this field is a LONG. It contains the number of minutes to add to the value of the Bias field during standard time. 
            //DaylightName (64 bytes): The value of this field is an array of 32 WCHARs.It contains an optional description for DST.Any unused WCHARs in the array MUST be set to 0x0000. 
            //DaylightDate (16 bytes): The value of this field is a SYSTEMTIME structure. It contains the date and time when the transition from standard time to DST occurs. 
            //DaylightBias (4 bytes): The value of this field is a LONG. It contains the number of minutes to add to the value of the Bias field during DST.
            //The TimeZone structure is encoded using base64 encoding prior to being inserted in an XML element.Elements with a TimeZone structure MUST be encoded and transmitted as [WBXML1.2] inline strings. 
            var ByteItems = new byte[172];

            // Bias conversion
            var BiasByte = new byte[4];
            int j;
            try
            {
                ByteItems = System.Convert.FromBase64String(TimeZoneText.Text);
            }
            catch (System.Exception ex) {

                string message = "An Error Occurred.. May be data provided is not in expected format:" + "\r\n"+"\n" + ex.Message + "\r\n" ;
                string caption = "Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;
                result = MessageBox.Show(message, caption, buttons);
            }

            j = 0;

            if (ByteItems.Length >= 172)
            {


                for (int i = 0; i < 4; i++)
                {
                    BiasByte[j] = ByteItems[i];
                    j = j + 1;
                }
                int TimeZoneBias = BitConverter.ToInt32(BiasByte, 0); // Byte to Signed Integer/Decimal conversion
                ConversionResult.Text = "Bias : " + TimeZoneBias.ToString() + "\r\n";//CharArray.GetValue(i).ToString();

                // StandardName conversion
                var StandardNameByte = new byte[64];
                j = 0;
                for (int i = 4; i < 68; i++)
                {
                    StandardNameByte[j] = ByteItems[i];
                    j = j + 1;
                }
                string TZStandardName = System.Text.Encoding.UTF8.GetString(StandardNameByte, 0, StandardNameByte.Length);
                TZStandardName = TZStandardName.Replace("\0", "");
                ConversionResult.Text += "StandardName : " + TZStandardName + "\r\n";


                // StandardDate conversion
                var StandardDateByte = new byte[16];
                j = 0;
                for (int i = 68; i < 84; i++)
                {

                    StandardDateByte[j] = ByteItems[i];
                    j = j + 1;

                }
                var wYear = new byte[2];
                wYear[0] = StandardDateByte[0];
                wYear[1] = StandardDateByte[1];

                var wMonth = new byte[2];
                wMonth[0] = StandardDateByte[2];
                wMonth[1] = StandardDateByte[3];

                var wDayOfWeek = new byte[2];
                wDayOfWeek[0] = StandardDateByte[4];
                wDayOfWeek[1] = StandardDateByte[5];

                var wDay = new byte[2];
                wDay[0] = StandardDateByte[6];
                wDay[1] = StandardDateByte[7];

                var wHour = new byte[2];
                wHour[0] = StandardDateByte[8];
                wHour[1] = StandardDateByte[9];

                var wMinute = new byte[2];
                wMinute[0] = StandardDateByte[10];
                wMinute[1] = StandardDateByte[11];


                var wSecond = new byte[2];
                wSecond[0] = StandardDateByte[12];
                wSecond[1] = StandardDateByte[13];

                var wMilliseconds = new byte[2];
                wMilliseconds[0] = StandardDateByte[14];
                wMilliseconds[1] = StandardDateByte[15];

                string TZStandardDate = "Year : " + BitConverter.ToInt16(wYear, 0) + "," + "Month : " + BitConverter.ToInt16(wMonth, 0) + "," + "DayOfWeek : " + BitConverter.ToInt16(wDayOfWeek, 0) + "," + "Day : " + BitConverter.ToInt16(wDay, 0) + "," + "Hour : " + BitConverter.ToInt16(wHour, 0) + "," + "Minute : " + BitConverter.ToInt16(wMinute, 0) + "," + "Second : " + BitConverter.ToInt16(wSecond, 0) + "," + "MilliSeconds : " + BitConverter.ToInt16(wMilliseconds, 0);
                TZStandardDate = TZStandardDate.Replace("\0", "");
                ConversionResult.Text += "StandardDate : " + TZStandardDate + "\r\n";

                //StandardBias conversion
                var StandardBiasByte = new byte[4];
                j = 0;
                for (int i = 84; i < 88; i++)
                {
                    StandardBiasByte[j] = ByteItems[i];
                    j = j + 1;
                }
                int TimeZoneStandardBias = BitConverter.ToInt32(StandardBiasByte, 0); // Byte to Signed Integer/Decimal conversion
                ConversionResult.Text += "StandardBias : " + TimeZoneStandardBias.ToString() + "\r\n";//CharArray.GetValue(i).ToString();

                // DaylightName conversion
                var DaylightNameByte = new byte[64];
                j = 0;
                for (int i = 88; i < 152; i++)
                {
                    DaylightNameByte[j] = ByteItems[i];
                    j = j + 1;
                }
                string TZDaylightName = System.Text.Encoding.UTF8.GetString(DaylightNameByte, 0, DaylightNameByte.Length);
                TZDaylightName = TZDaylightName.Replace("\0", "");
                ConversionResult.Text += "DaylightName : " + TZDaylightName + "\r\n";

                var DaylightDateByte = new byte[16];
                j = 0;
                for (int i = 152; i < 168; i++)
                {

                    DaylightDateByte[j] = ByteItems[i];
                    j = j + 1;

                }
                wYear = new byte[2];
                wYear[0] = DaylightDateByte[0];
                wYear[1] = DaylightDateByte[1];

                wMonth = new byte[2];
                wMonth[0] = DaylightDateByte[2];
                wMonth[1] = DaylightDateByte[3];

                wDayOfWeek = new byte[2];
                wDayOfWeek[0] = DaylightDateByte[4];
                wDayOfWeek[1] = DaylightDateByte[5];

                wDay = new byte[2];
                wDay[0] = DaylightDateByte[6];
                wDay[1] = DaylightDateByte[7];

                wHour = new byte[2];
                wHour[0] = DaylightDateByte[8];
                wHour[1] = DaylightDateByte[9];

                wMinute = new byte[2];
                wMinute[0] = DaylightDateByte[10];
                wMinute[1] = DaylightDateByte[11];

                wSecond = new byte[2];
                wSecond[0] = DaylightDateByte[12];
                wSecond[1] = DaylightDateByte[13];

                wMilliseconds = new byte[2];
                wMilliseconds[0] = DaylightDateByte[14];
                wMilliseconds[1] = DaylightDateByte[15];

                string TZDaylightDate = "Year : " + BitConverter.ToInt16(wYear, 0) + "," + "Month : " + BitConverter.ToInt16(wMonth, 0) + "," + "DayOfWeek : " + BitConverter.ToInt16(wDayOfWeek, 0) + "," + "Day : " + BitConverter.ToInt16(wDay, 0) + "," + "Hour : " + BitConverter.ToInt16(wHour, 0) + "," + "Minute : " + BitConverter.ToInt16(wMinute, 0) + "," + "Second : " + BitConverter.ToInt16(wSecond, 0) + "," + "MilliSeconds : " + BitConverter.ToInt16(wMilliseconds, 0);
                TZDaylightDate = TZDaylightDate.Replace("\0", "");
                ConversionResult.Text += "DaylightDate : " + TZDaylightDate + "\r\n";

                // DaylightBiasByte conversion
                var DaylightBiasByte = new byte[4];
                j = 0;
                for (int i = 168; i < 172; i++)
                {
                    DaylightBiasByte[j] = ByteItems[i];
                    j = j + 1;
                }
                int TimeZoneDaylightBias = BitConverter.ToInt32(DaylightBiasByte, 0); // Byte to Signed Integer/Decimal conversion
                ConversionResult.Text += "DaylightBias : " + TimeZoneDaylightBias.ToString();//CharArray.GetValue(i).ToString();
            }
            else
            {

                string message = "An Error Occurred.." + "\r\n" + "\r\n" + "Data you provided is less than 172 bytes. Data should be at least 172 bytes for ActiveSync TimeZone." + "\r\n" + "\r\n" + "Please refer this protocol documentation: http://interoperability.blob.core.windows.net/files/MS-ASDTYPE/[MS-ASDTYPE].pdf ." +"\r\n";
                string caption = "Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;
                result = MessageBox.Show(message, caption, buttons);

            }
        } //button click

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "This is written for CSS ActiveSync cases time zone conversions."+ "\r\n" + "Simple tool written by kekici@microsoft.com." + "\r\n"+ "\r\n" + "TimeZone info should be collected from EAS trace(fiddler,mailbox debug, extrace etc..) and result; you can copy and paste (ctrl+c / v)." +"\r\n" + "\r\n" + "Referans document for TimeZone information: http://interoperability.blob.core.windows.net/files/MS-ASDTYPE/[MS-ASDTYPE].pdf" +"\r\n"+"\n";
            string caption = "About It";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;
            result = MessageBox.Show(message, caption, buttons);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

 
}
