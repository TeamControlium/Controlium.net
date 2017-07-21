﻿using System;
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
    [Flags]
    public enum UserCredentialsDialogFlags
    {
        Default = GenericCredentials |
                    ShowSaveCheckbox |
                    AlwaysShowUI |
                    ExpectConfirmation,
        None = 0x0,
        IncorrectPassword = 0x1,
        DoNotPersist = 0x2,
        RequestAdministrator = 0x4,
        ExcludesCertificates = 0x8,
        RequireCertificate = 0x10,
        ShowSaveCheckbox = 0x40,
        AlwaysShowUI = 0x80,
        RequireSmartCard = 0x100,
        PasswordOnlyOk = 0x200,
        ValidateUsername = 0x400,
        CompleteUserName = 0x800,
        Persist = 0x1000,
        ServerCredential = 0x4000,
        ExpectConfirmation = 0x20000,
        GenericCredentials = 0x40000,
        UsernameTargetCredentials = 0x80000,
        KeepUsername = 0x100000
    }

    /// <summary>
    /// Displays a dialog box and prompts the user for login credentials.
    /// </summary>
    [ToolboxItem(true)]
    [DesignerCategory("Dialogs")]
    public class UserCredentialsDialog : CommonDialog
    {


        private string user;
        private SecureString password;
        //private string domain;
        private string target;
        private string message;
        private string caption;
        // private Image banner;
        private bool saveChecked;
        private UserCredentialsDialogFlags flags;

        public UserCredentialsDialog()
        {
            this.Reset();
        }


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


        public bool SaveChecked
        {
            get { return saveChecked; }
            set { saveChecked = value; }
        }

        public UserCredentialsDialogFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        public void ConfirmCredentials(bool confirm)
        {
            new UIPermission(UIPermissionWindow.SafeSubWindows).Demand();

            Win32Native.CredUIReturnCodes result = Win32Native.CredUIConfirmCredentialsW(this.target, confirm);

            if (result != Win32Native.CredUIReturnCodes.NO_ERROR &&
                result != Win32Native.CredUIReturnCodes.ERROR_NOT_FOUND &&
                result != Win32Native.CredUIReturnCodes.ERROR_INVALID_PARAMETER)
            {
                throw new InvalidOperationException(TranslateReturnCode(result));
            }
        }


        /// <summary>
        /// This method is for backward compatibility with APIs that does
        /// not provide the <see cref="SecureString"/> type.
        /// </summary>
        /// <returns></returns>
        public string PasswordToString()
        {
            IntPtr ptr = Marshal.SecureStringToGlobalAllocUnicode(this.password);
            try
            {
                // Unsecure managed string
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }

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
                pwd.Append(this.PasswordToString());
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
        /// Set all properties to it's default values.
        /// </summary>
        public override void Reset()
        {
            this.target = Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName;
            this.user = null;
            this.password = null;
            this.caption = null;// target as caption;
            this.message = null;
            this.saveChecked = false;
            this.flags = UserCredentialsDialogFlags.Default;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this.password != null)
            {
                this.password.Dispose();
                this.password = null;
            }
        }


        private static string TranslateReturnCode(Win32Native.CredUIReturnCodes result)
        {
            return string.Format("Invalid operation: {0}", result.ToString());
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
