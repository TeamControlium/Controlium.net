using System;
using System.Text;
using System.Drawing;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace TeamControlium.Framework
{

    /// <summary>
    /// Displays a dialog box and prompts the user for login credentials.
    /// </summary>
    /// <example>
    /// <code language="C#" title="Usage">
    /// <![CDATA[
    /// UserCredentialsDialog ucd = new UserCredentialsDialog();
    /// ucd.Caption = "Test Automation";
    /// ucd.Message = "Enter username/password of user able to access the xyz resource.";
    /// ucd.ConfirmCredentials(true);
    /// ucd.User = "MyUser";  //optional
    /// ucd.ShowDialog();
    /// bool SaveTicked = ucd.SaveChecked;
    /// string user = ucd.User;
    /// string pass = ucd.PasswordToString();]]>
    /// </code>
    /// </example>
    [ToolboxItem(true)]
    [DesignerCategory("Dialogs")]
    public class UserCredentialsDialog : CommonDialog
    {
        /// <summary>
        /// Flags used and set by Credentials Dialog
        /// </summary>
        [Flags]
        public enum UserCredentialsDialogFlags
        {
            /// <summary>
            /// GenericCredentials, ShowSaveCheckbox, AlwaysShowUI and ExpectConfirmation
            /// </summary>
            Default = GenericCredentials |
                        ShowSaveCheckbox |
                        AlwaysShowUI |
                        ExpectConfirmation,
            None = 0x0,
            /// <summary>
            /// Notify the user of insufficient credentials by displaying the "Logon unsuccessful" balloon tip.
            /// </summary>
            IncorrectPassword = 0x1,
            /// <summary>
            /// Do not store credentials or display check boxes. You can use ShowSaveCheckbox with this flag to display the Save check box only, and the result is returned in the SaveChecked property.
            /// </summary>
            DoNotPersist = 0x2,
            /// <summary>
            /// Populate the combo box with local administrators only.
            /// </summary>
            RequestAdministrator = 0x4,
            /// <summary>
            /// Populate the combo box with user name/password only. Do not display certificates or smart cards in the combo box.
            /// </summary>
            ExcludesCertificates = 0x8,
            /// <summary>
            /// Populate the combo box with certificates and smart cards only. Do not allow a user name to be entered.
            /// </summary>
            RequireCertificate = 0x10,
            /// <summary>
            /// If the check box is selected, show the Save check box and return TRUE in the SaveCheckbox property, otherwise, return FALSE. Check box uses the value in SaveCheckbox by default.
            /// </summary>
            ShowSaveCheckbox = 0x40,
            /// <summary>
            /// Specifies that a user interface will be shown even if the credentials can be returned from an existing credential in credential manager. This flag is permitted only if GenericCredentials is also specified.
            /// </summary>
            AlwaysShowUI = 0x80,
            /// <summary>
            /// Populate the combo box with certificates or smart cards only. Do not allow a user name to be entered.
            /// </summary>
            RequireSmartCard = 0x100,
            /// <summary>
            /// Populate the combo box with the password only. Do not allow a user name to be entered.
            /// </summary>
            PasswordOnlyOk = 0x200,
            /// <summary>
            /// Check that the user name is valid.
            /// </summary>
            ValidateUsername = 0x400,
            /// <summary>
            /// Populate the combo box with the prompt for a user name.
            /// </summary>
            CompleteUserName = 0x800,
            /// <summary>
            /// Do not show the Save check box, but the credential is saved as though the box were shown and selected.
            /// </summary>
            Persist = 0x1000,
            /// <summary>
            /// Wildcard credentials will not be matched.
            /// </summary>
            /// <remarks>
            /// This flag is meaningful only in locating a matching credential to prefill the dialog box, should authentication fail. It has no effect when writing a credential. CredUI does not create credentials that contain wildcard characters. Any found were either created explicitly by the user or created programmatically, as happens when a RAS connection is made.
            /// </remarks>
            ServerCredential = 0x4000,
            /// <summary>
            /// Specifies that the caller will call CredUIConfirmCredentials after checking to determine whether the returned credentials are actually valid. This mechanism ensures that credentials that are not valid are not saved to the credential manager. Specify this flag in all cases unless Persist is specified.
            /// </summary>
            ExpectConfirmation = 0x20000,
            /// <summary>
            /// Consider the credentials entered by the user to be generic credentials.
            /// </summary>
            GenericCredentials = 0x40000,
            /// <summary>
            /// The credential is a "runas" credential. The TargetName parameter specifies the name of the command or program being run. It is used for prompting purposes only.
            /// </summary>
            UsernameTargetCredentials = 0x80000,
            /// <summary>
            /// Do not allow the user to change the supplied user name.
            /// </summary>
            KeepUsername = 0x100000
        }

        private string user;
        private SecureString password;
        private string passwordIn;
        private string target;
        private string message;
        private string caption;
        private bool saveChecked;
        private UserCredentialsDialogFlags flags;

        /// <summary>
        /// Instantiate the Credentials Dialog
        /// </summary>
        public UserCredentialsDialog()
        {
            this.Reset();
        }

        /// <summary>
        /// Pre-populates Username textbox when dialog is shown and returns username after OK hit.  Null if cancelled.
        /// </summary>
        public string User
        {
            get { return user; }
            set
            {
                if (value != null)
                {
                    if (value.Length > Win32Native.CREDUI_MAX_USERNAME_LENGTH)
                    {
                        throw new ArgumentException(string.Format(
                            "The user name has a maximum length of {0} characters.",
                            Win32Native.CREDUI_MAX_USERNAME_LENGTH), "User");
                    }
                }
                user = value;
            }
        }
        /// <summary>
        /// Pre-populates password with secure string when dialog is shown and returns secure password after OK hit.  Null if cancelled.
        /// </summary>
        public SecureString Password
        {
            get { return password; }
            set
            {
                if (value != null)
                {
                    if (value.Length > Win32Native.CREDUI_MAX_PASSWORD_LENGTH)
                    {
                        throw new ArgumentException(string.Format(
                            "The password has a maximum length of {0} characters.",
                            Win32Native.CREDUI_MAX_PASSWORD_LENGTH), "Password");
                    }
                }
                password = value;
            }
        }
        /// <summary>
        /// Message shown in dialog
        /// </summary>
        public string Message
        {
            get { return message; }
            set
            {
                if (value != null)
                {
                    if (value.Length > Win32Native.CREDUI_MAX_MESSAGE_LENGTH)
                    {
                        throw new ArgumentException(
                            string.Format("The message has a maximum length of {0} characters.",
                            Win32Native.CREDUI_MAX_MESSAGE_LENGTH), "Message");
                    }
                }
                message = value;
            }
        }
        /// <summary>
        /// Caption of dialog
        /// </summary>
        public string Caption
        {
            get { return caption; }
            set
            {
                if (value != null)
                {
                    if (value.Length > Win32Native.CREDUI_MAX_CAPTION_LENGTH)
                    {
                        throw new ArgumentException(
                            string.Format("The caption has a maximum length of {0} characters.",
                            Win32Native.CREDUI_MAX_CAPTION_LENGTH), "Caption");
                    }
                }
                caption = value;
            }
        }

        /// <summary>
        /// Indicates if Save checkbox has been ticked ot not (See UserCredentialsDialogFlags)
        /// </summary>
        public bool SaveChecked
        {
            get { return saveChecked; }
            set { saveChecked = value; }
        }
        /// <summary>
        /// Flags for use when dialog is shown (See UserCredentialsDialogFlags)
        /// </summary>
        public UserCredentialsDialogFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        /// <summary>
        /// Store user credentials in the Credential Manager.
        /// </summary>
        /// <param name="confirm"> If TRUE, the credentials are stored in the credential manager as defined by CredUIPromptForCredentials or CredUICmdLinePromptForCredentials. If FALSE, the credentials are not stored and various pieces of memory are cleaned up.</param>
        public void ConfirmCredentials(bool confirm)
        {
            (new UIPermission(UIPermissionWindow.SafeSubWindows)).Demand();

            Win32Native.CredUIReturnCodes result = Win32Native.CredUIConfirmCredentialsW(this.target, confirm);

            if (result != Win32Native.CredUIReturnCodes.NO_ERROR &&
                result != Win32Native.CredUIReturnCodes.ERROR_NOT_FOUND &&
                result != Win32Native.CredUIReturnCodes.ERROR_INVALID_PARAMETER)
            {
                throw new InvalidOperationException(TranslateReturnCode(result));
            }
        }

        /// <summary>
        /// This method is for backward compatibility with APIs that do
        /// not provide the SecureString type.
        /// </summary>
        /// <returns>Decripted plain text password</returns>
        public string PasswordAsString
        {
            get
            {
                IntPtr ptr = default(IntPtr);
                try
                {
                    ptr = Marshal.SecureStringToGlobalAllocUnicode(this.password);
                    // Unsecure managed string
                    return Marshal.PtrToStringUni(ptr);
                }
                catch
                {
                    return null;
                }
                finally
                {
                    if (ptr != default(IntPtr)) Marshal.ZeroFreeGlobalAllocUnicode(ptr);
                }
            }
            set
            {
                passwordIn = value;
            }
        }
 
        /// <summary>
        /// Set all properties to it's default values.
        /// </summary>
        public override void Reset()
        {
            this.target = Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName;
            this.user = null;
            this.password = null;
            this.passwordIn = null;
            this.caption = null;// target as caption;
            this.message = null;
            this.saveChecked = false;
            this.flags = UserCredentialsDialogFlags.Default;
        }

        /// <summary>
        /// Do Not Use.
        /// </summary>
        /// <param name="hwndOwner">handle</param>
        /// <returns>bool</returns>
        protected override bool RunDialog(IntPtr hwndOwner)
        {

            Win32Native.CredUIInfo credInfo = new Win32Native.CredUIInfo(hwndOwner,
                this.caption, this.message, null);
            StringBuilder usr = new StringBuilder(Win32Native.CREDUI_MAX_USERNAME_LENGTH);
            StringBuilder pwd = new StringBuilder(Win32Native.CREDUI_MAX_PASSWORD_LENGTH);

            if (!string.IsNullOrEmpty(this.User))
            {

                usr.Append(this.User);
            }
            if (this.Password != null)
            {
                pwd.Append(this.PasswordAsString);
            }
            else
            {
                if (this.passwordIn != null)
                {
                    pwd.Append(this.passwordIn);
                }
            }

            try
            {
                Win32Native.CredUIReturnCodes result = Win32Native.CredUIPromptForCredentials(
                                                        ref credInfo, this.target,
                                                        IntPtr.Zero, 0,
                                                        usr, Win32Native.CREDUI_MAX_USERNAME_LENGTH,
                                                        pwd, Win32Native.CREDUI_MAX_PASSWORD_LENGTH,
                                                        ref this.saveChecked, this.flags);
                switch (result)
                {
                    case Win32Native.CredUIReturnCodes.NO_ERROR:
                        LoadUserDomainValues(usr);
                        LoadPasswordValue(pwd);
                        return true;
                    case Win32Native.CredUIReturnCodes.ERROR_CANCELLED:
                        this.User = null;
                        this.Password = null;
                        return false;
                    default:
                        throw new InvalidOperationException(TranslateReturnCode(result));
                }
            }
            finally
            {
                usr.Remove(0, usr.Length);
                pwd.Remove(0, pwd.Length);

            }
        }
 
        /// <summary>
        /// Do not use
        /// </summary>
        /// <param name="disposing">Indicates if disposing</param>
        protected override void Dispose(bool disposing)
        {
            //
            // We make sure the password has been correctly disposed for security!  GC may run an a finite time after disposing dialog and so we kinda force the issue.
            //
            base.Dispose(disposing);

            if (this.password != null)
            {
                this.password.Dispose();
                this.password = null;
            }
        }

        private void LoadPasswordValue(StringBuilder password)
        {
            char[] pwd = new char[password.Length];
            SecureString securePassword = new SecureString();
            try
            {
                password.CopyTo(0, pwd, 0, pwd.Length);
                foreach (char c in pwd)
                {
                    securePassword.AppendChar(c);
                }
                securePassword.MakeReadOnly();
                this.Password = securePassword.Copy();
            }
            finally
            {
                // discard the char array
                Array.Clear(pwd, 0, pwd.Length);
            }
        }

        private void LoadUserDomainValues(StringBuilder principalName)
        {
            StringBuilder user = new StringBuilder(Win32Native.CREDUI_MAX_USERNAME_LENGTH);
            StringBuilder domain = new StringBuilder(Win32Native.CREDUI_MAX_DOMAIN_TARGET_LENGTH);
            Win32Native.CredUIReturnCodes result = Win32Native.CredUIParseUserNameW(principalName.ToString(),
                user, Win32Native.CREDUI_MAX_USERNAME_LENGTH, domain, Win32Native.CREDUI_MAX_DOMAIN_TARGET_LENGTH);

            if (result == Win32Native.CredUIReturnCodes.NO_ERROR)
            {
                this.User = user.ToString();
                // this.Domain = domain.ToString();
            }
            else
            {
                this.User = principalName.ToString();
                // this.Domain = Environment.MachineName;
            }
        }

        private static string TranslateReturnCode(Win32Native.CredUIReturnCodes result)
        {
            return string.Format("Invalid operation: {0}", result.ToString());
        }

        [SuppressUnmanagedCodeSecurity]
        private sealed class Win32Native
        {
            internal const int CREDUI_MAX_MESSAGE_LENGTH = 100;
            internal const int CREDUI_MAX_CAPTION_LENGTH = 100;
            internal const int CREDUI_MAX_GENERIC_TARGET_LENGTH = 100;
            internal const int CREDUI_MAX_DOMAIN_TARGET_LENGTH = 100;
            internal const int CREDUI_MAX_USERNAME_LENGTH = 100;
            internal const int CREDUI_MAX_PASSWORD_LENGTH = 100;
            internal const int CREDUI_BANNER_HEIGHT = 60;
            internal const int CREDUI_BANNER_WIDTH = 320;

            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            internal static extern bool DeleteObject(IntPtr hObject);

            [DllImport("credui.dll", EntryPoint = "CredUIPromptForCredentialsW", SetLastError = true, CharSet = CharSet.Unicode)]
            internal extern static CredUIReturnCodes CredUIPromptForCredentials(
                ref CredUIInfo creditUR,
                string targetName,
                IntPtr reserved1,
                int iError,
                StringBuilder userName,
                int maxUserName,
                StringBuilder password,
                int maxPassword,
                ref bool iSave,
                UserCredentialsDialogFlags flags);

            [DllImport("credui.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal extern static CredUIReturnCodes CredUIParseUserNameW(
                string userName,
                StringBuilder user,
                int userMaxChars,
                StringBuilder domain,
                int domainMaxChars);

            [DllImport("credui.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal extern static CredUIReturnCodes CredUIConfirmCredentialsW(string targetName, bool confirm);

            internal enum CredUIReturnCodes
            {
                NO_ERROR = 0,
                ERROR_CANCELLED = 1223,
                ERROR_NO_SUCH_LOGON_SESSION = 1312,
                ERROR_NOT_FOUND = 1168,
                ERROR_INVALID_ACCOUNT_NAME = 1315,
                ERROR_INSUFFICIENT_BUFFER = 122,
                ERROR_INVALID_PARAMETER = 87,
                ERROR_INVALID_FLAGS = 1004
            }

            internal struct CredUIInfo
            {
                internal CredUIInfo(IntPtr owner, string caption, string message, Image banner)
                {
                    this.cbSize = Marshal.SizeOf(typeof(CredUIInfo));
                    this.hwndParent = owner;
                    this.pszCaptionText = caption;
                    this.pszMessageText = message;

                    if (banner != null)
                    {
                        this.hbmBanner = new Bitmap(banner,
                            Win32Native.CREDUI_BANNER_WIDTH, Win32Native.CREDUI_BANNER_HEIGHT).GetHbitmap();
                    }
                    else
                    {
                        this.hbmBanner = IntPtr.Zero;
                    }
                }

                internal int cbSize;
                internal IntPtr hwndParent;
                [MarshalAs(UnmanagedType.LPWStr)]
                internal string pszMessageText;
                [MarshalAs(UnmanagedType.LPWStr)]
                internal string pszCaptionText;
                internal IntPtr hbmBanner;
            }
        }
    }

}
