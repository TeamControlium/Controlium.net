using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.Remoting;
using TeamControlium.Utilities;

namespace TeamControlium.Controlium
{
    public partial class SeleniumDriver
    {
        /// <summary>
        /// Supported Devices.  Device is set when Driver is instantiated.  Driver gets the Device to be used from the run configuration
        /// option "Device" in category "General".  Windows Vista was considered a joke and is not supported by Cloud providers (at this time or writing).  iPhones
        /// may have a number of different iOS versions installed and so it is the iOS version that is set; see the Devices for the assumed host iPhone.
        /// <para/><para/>
        /// When running remote Selenium the browser/version is used in the Desired Capabilities to request
        /// that device (or device simulator). 
        /// <para/><para/>
        /// When running locally the selected device is ignored and Windows 7 assumed.
        /// </summary>
        public enum Devices
        {
            /// <summary>
            /// No Device has been selected.
            /// </summary>
            NoneSelected,
            /// <summary>
            /// Windows XP (Latest Service pack is assumed)
            /// <para/>
            /// Valid Config options; 'XP', 'Windows XP'
            /// </summary>
            WinXP,
            /// <summary>
            /// Windows 7 (Latest Service pack is assumed)
            /// <para/>
            /// Valid Config options; 'Windows 7', 'Win 7', 'W7'
            /// </summary>
            Win7,
            /// <summary>
            /// Windows 8 (Latest Service pack is assumed)
            /// <para/>
            /// Valid Config options; 'Windows 8', 'Win 8', 'W8', 'Windows 8.0', 'Win 8.0', 'W8.0', 'Windows 80', 'Win 80', 'W80','Windows 8_0', 'Win 8_0', 'W8_0'
            /// </summary>
            Win8,
            /// <summary>
            /// Windows 8.1 (Latest Service pack is assumed)
            /// <para/>
            /// Valid Config options; 'Windows 8.1', 'Win 8.1', 'W8.1', 'Windows 81', 'Win 81', 'W81','Windows 8_1', 'Win 8_1', 'W8_1'
            /// </summary>            
            Win8_1,
            /// <summary>
            /// Windows 10 (Latest Service pack is assumed)
            /// <para/>
            /// Valid Config options; 'Windows 10', 'Win 10', 'W10'
            /// </summary>            
            Win10,
            /// <summary>
            /// iPad 1
            /// <para/>
            /// Valid Config options; 'iPad', 'iPad 1', 'a1219', 'a1337'
            /// </summary>
            iPad,
            /// <summary>
            /// iPad 2
            /// <para/>
            /// Valid Config options; 'iPad 2', 'a1395', 'a1396', 'a1397'
            /// </summary>
            iPad2,
            /// <summary>
            /// iPad 3
            /// <para/>
            /// Valid Config options; 'iPad 3', 'iPad 3rd Gen, 'iPad 3rd Generation', 'a1416', 'a1430', 'a1403'
            /// </summary>
            iPad3,
            /// <summary>
            /// iPad 4
            /// <para/>
            /// Valid Config options; 'iPad 4', 'iPad 4th Gen, 'iPad 4th Generation', 'a1458', 'a1459', 'a1460'
            /// </summary>
            iPad4,
            /// <summary>
            /// iPad Mini
            /// <para/>
            /// Valid Config options; 'iPad Mini', 'iPad Mini 1', 'a1432', 'a1454', 'a1455'
            /// </summary>
            iPadMini,
            /// <summary>
            /// iPad Mini 2
            /// <para/>
            /// Valid Config options; 'iPad M2', 'iPad Mini 2', 'a1489', 'a1490', 'a1491'
            /// </summary>
            iPadMini2,
            /// <summary>
            /// iPad Mini 3
            /// <para/>
            /// Valid Config options; 'iPad M3', 'iPad Mini 3', 'a1599', 'a1600'
            /// </summary>
            iPadMini3,
            /// <summary>
            /// iPad Air
            /// <para/>
            /// Valid Config options; 'iPad Air', 'iPad Air 1', 'a1474', 'a1475', 'a1476
            /// </summary>
            iPadAir,
            /// <summary>
            /// iPad Air 2
            /// <para/>
            /// Valid Config options; 'iPad Air 2', 'a1566', 'a1567'
            /// </summary>
            iPadAir2,
            /// <summary>
            /// iPhone running iOS 1. This is only found iPhone 1st Generation devices
            /// <para/>
            /// Valid Config options; 'iOS', 'iOS 1', 'iPhone OS', 'iPhone OS 1'
            /// </summary>
            iOS1,
            /// <summary>
            /// iPhone running iOS 2. This is only found on the iPhone 3G device
            /// <para/>
            /// Valid Config options; 'iOS 2', 'iPhone OS 2'
            /// </summary>
            iOS2,
            /// <summary>
            /// iPhone running iOS 3. This is only found on the iPhone 3GS device
            /// <para/>
            /// Valid Config options; 'iOS 3', 'iPhone OS 3'
            /// </summary>
            iOS3,
            /// <summary>
            /// iPhone running iOS 4. This is found on iPhone 3G, 3GS and iPhone 4.  If set, it assumed the device is an iPhone 4
            /// <para/>
            /// Valid Config options; 'iOS 4', 'iPhone OS 4'
            /// </summary>
            iOS4,
            /// <summary>
            /// iPhone running iOS 5. This is found on iPhone 3GS, 4 and 4S. If set, it assumed the device is an iPhone 4S
            /// <para/>
            /// Valid Config options; 'iOS 5', 'iPhone OS 5'
            /// </summary>
            iOS5,
            /// <summary>
            /// iPhone running iOS 6. This is found on iPhone 3GS, 4, 4S and 5. If set, it assumed the device is an iPhone 5
            /// <para/>
            /// Valid Config options; 'iOS 6', 'iPhone OS 6'
            /// </summary>
            iOS6,
            /// <summary>
            /// iPhone running iOS 7. This is found on the iPhone 4, 4S, 5, 5C and 5S devices. If set, it assumed the device is an iPhone 5
            /// <para/>
            /// Valid Config options; 'iOS 7', 'iPhone OS 7'
            /// </summary>
            iOS7,
            /// <summary>
            /// iPhone running iOS 8. This is found on iPhone 4S, 5, 5C, 5S, 6 and 6 Plus devices. If set, it assumed the device is an iPhone 6 Plus
            /// <para/>
            /// Valid Config options; 'iOS 8', 'iPhone OS 8'
            /// </summary>
            iOS8,
            /// <summary>
            /// Samsung Galaxy S.
            /// <para/>
            /// Valid Config options; 'S', 'S1', 'Si', 'Galaxy', 'Galaxy 1', 'Galaxy S', 'Galaxy S1', 'i9000', 'i9001', 'i9070'
            /// </summary>
            GalaxyS,
            /// <summary>
            /// Samsung Galaxy S2.
            /// <para/>
            /// Valid Config options; 'S2', 'Sii', 'Galaxy 2', 'Galaxy S2', 'i9100', 'i9100g', 'i9105'
            /// </summary>
            GalaxyS2,
            /// <summary>
            /// Samsung Galaxy S3.
            /// <para/>
            /// Valid Config options; 'S3', 'Siii', 'Galaxy 3', 'Galaxy S3', 'i9300', 'i9305'
            /// </summary>
            GalaxyS3,
            /// <summary>
            /// Samsung Galaxy S4.
            /// <para/>
            /// Valid Config options; 'S4', 'Galaxy 4', 'Galaxy S4', 'i9500', 'i9505'
            /// </summary>
            GalaxyS4,
            /// <summary>
            /// Samsung Galaxy S5.
            /// <para/>
            /// Valid Config options; 'S5', 'Galaxy 5', 'Galaxy S5', 'g900f', 'g800f', 'g900h', 'smg900f', 'smg800f', 'smg900h', 'sm-g900f', 'sm-g800f', 'sm-g900h'
            /// </summary>
            GalaxyS5
        }

        /// <summary>
        /// Device hosting browser Selenium script executing against
        /// </summary>
        /// <seealso cref="Devices">Lists all possible Devices that can be returned.</seealso>
        public Devices TestDevice { get; private set; }

        private void SetTestDevice()
        {
            string device = TestData.Repository[ConfigDevice[0], ConfigDevice[1]];

            if (string.IsNullOrEmpty(device)) throw new RunOptionOrCategoryDoesNotExist(ConfigDevice[0], ConfigDevice[1], "Unable to set Selenium host Device.");
            switch (device.ToLower().Replace(" ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty))
            {
                case "xp":
                case "windowsxp":
                    TestDevice = Devices.WinXP; break;
                case "windows7":
                case "win7":
                case "w7":
                    TestDevice = Devices.Win7; break;
                case "windows8":
                case "win8":
                case "w8":
                case "windows8.0":
                case "win8.0":
                case "w8.0":
                case "windows80":
                case "win80":
                case "w80":
                case "windows8_0":
                case "win8_0":
                case "w8_0":
                    TestDevice = Devices.Win8; break;
                case "windows8.1":
                case "win8.1":
                case "w8.1":
                case "windows81":
                case "win81":
                case "w81":
                case "windows8_1":
                case "win8_1":
                case "w8_1":
                    TestDevice = Devices.Win8_1; break;
                case "windows10":
                case "win10":
                case "w10":
                    TestDevice = Devices.Win10; break;
                case "ipad":
                case "ipad1":
                case "a1219":
                case "a1337":
                    TestDevice = Devices.iPad; break;
                case "ipad2":
                case "a1395":
                case "a1396":
                case "a1397":
                    TestDevice = Devices.iPad2; break;
                case "ipad3":
                case "ipad3rdgen":
                case "ipad3rdgeneration":
                case "a1416":
                case "a1430":
                case "a1403":
                    TestDevice = Devices.iPad3; break;
                case "ipad4":
                case "ipad4thgen":
                case "ipad4thgeneration":
                case "a1458":
                case "a1459":
                case "a1460":
                    TestDevice = Devices.iPad4; break;
                case "ipadmini":
                case "ipadmini1":
                case "a1432":
                case "a1454":
                case "a1455":
                    TestDevice = Devices.iPadMini; break;
                case "ipadm2":
                case "ipadmini2":
                case "a1489":
                case "a1490":
                case "a1491":
                    TestDevice = Devices.iPadMini2; break;
                case "ipadm3":
                case "ipadmini3":
                case "a1599":
                case "a1600":
                    TestDevice = Devices.iPadMini3; break;
                case "ipadair":
                case "ipadair1":
                case "a1474":
                case "a1475":
                case "a1476":
                    TestDevice = Devices.iPadAir; break;
                case "ipadair2":
                case "a1566":
                case "a1567":
                    TestDevice = Devices.iPadAir2; break;
                case "ios":
                case "ios1":
                case "iphoneos":
                case "iphoneos1":
                    TestDevice = Devices.iOS1; break;
                case "ios2":
                case "iphoneos2":
                    TestDevice = Devices.iOS2; break;
                case "ios3":
                case "iphoneos3":
                    TestDevice = Devices.iOS3; break;
                case "ios4":
                case "iphoneos4":
                    TestDevice = Devices.iOS4; break;
                case "ios5":
                case "iphoneos5":
                    TestDevice = Devices.iOS5; break;
                case "ios6":
                case "iphoneos6":
                    TestDevice = Devices.iOS6; break;
                case "ios7":
                case "iphoneos7":
                    TestDevice = Devices.iOS7; break;
                case "ios8":
                case "iphoneos8":
                    TestDevice = Devices.iOS8; break;
                case "galaxy":
                case "galaxy1":
                case "galaxys":
                case "galaxys1":
                case "s":
                case "s1":
                case "si":
                case "i9000":
                case "i9001":
                case "i9070":
                    TestDevice = Devices.GalaxyS; break;
                case "galaxy2":
                case "galaxys2":
                case "s2":
                case "sii":
                case "i9100":
                case "i9100g":
                case "i9105":
                    TestDevice = Devices.GalaxyS2; break;
                case "galaxy3":
                case "galaxys3":
                case "s3":
                case "siii":
                case "i9300":
                case "i9305":
                    TestDevice = Devices.GalaxyS3; break;
                case "galaxy4":
                case "galaxys4":
                case "s4":
                case "i9500":
                case "i9505":
                    TestDevice = Devices.GalaxyS4; break;
                case "galaxy5":
                case "galaxys5":
                case "s5":
                case "g900f":
                case "g800f":
                case "g900h":
                case "smg900f":
                case "smg800f":
                case "smg900h":
                case "sm-g900f":
                case "sm-g800f":
                case "sm-g900h":
                    TestDevice = Devices.GalaxyS5; break;
                default:
                    throw new UnsupportedDevice(device);
            }
        }
    }

}
