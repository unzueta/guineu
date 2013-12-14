using System;
using System.Windows.Forms;
using Guineu.Expression;

namespace Guineu.Gui.Desktop
{
    class DesktopManager : WindowManager
    {
        public DesktopManager()
        {
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }
        void Application_ApplicationExit(object sender, EventArgs e)
        {
            GuineuInstance.Quit();
        }

        public override IControl CreateControl(KnownNti name)
        {
            switch (name)
            {
                case KnownNti.CheckBox:
                    return new DesktopCheckBox();
                case KnownNti.ComboBox:
                    return new DesktopComboBox();
                case KnownNti.CommandButton:
                    return new DesktopButton();
                case KnownNti.Container:
                    return new DesktopContainer();
                case KnownNti.EditBox:
                    return new DesktopEditbox { Multiline = true };
                case KnownNti.Form:
                    return new DesktopForm();
                case KnownNti.Image:
                    return new DesktopImage();
                case KnownNti.Label:
                    return new DesktopLabel();
                case KnownNti.ListBox:
                    return new DesktopListbox();
                case KnownNti.Page:
                    return new DesktopTabPage();
                case KnownNti.PageFrame:
                    return new DesktopPageframe();
                case KnownNti.Shape:
                    return new DesktopShape();
                case KnownNti.Spinner:
                    return new DesktopSpinner();
                case KnownNti.Timer:
                    return new DesktopTimer();
                case KnownNti.Textbox:
                    return new DesktopTextbox();
                default:
                    throw new ErrorException(ErrorCodes.ClassDefinitionNotFound);
            }
        }

        public override void ClearEvents()
        {
            Application.Exit();
        }
        public override void ReadEvents()
        {
            Application.Run();
        }

        //======================================================================================
        // window handling functions
        //======================================================================================
        public override Window GetWindowByName(String name)
        {
            // TODO: Access form collection
            return null;
        }
        public override Window CreateWindow()
        {
            return new GUIWindow(null);
        }

        public override TableOpenDialogResult ShowOpenTableDialog()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            TableOpenDialogResult result = new TableOpenDialogResult();
            if (dlg.ShowDialog().ToDialogResult() == DialogResult.OK)
                result.TableName = dlg.FileName;
            return result;
        }
        public override string ShowOpenFileDialog()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog().ToDialogResult() == DialogResult.OK)
                return dlg.FileName;
            return "";
        }
        public override ErrorAction ShowErrorDialog(string text)
        {
            System.Windows.Forms.DialogResult result =
                System.Windows.Forms.MessageBox.Show(
                    text,
                    "Execution Error",
                    System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore
                );

            switch (result)
            {
                case System.Windows.Forms.DialogResult.Abort:
                    return ErrorAction.Cancel;
                case System.Windows.Forms.DialogResult.Retry:
                    return ErrorAction.Retry;
                case System.Windows.Forms.DialogResult.Ignore:
                    return ErrorAction.Ignore;
            }
            return ErrorAction.Ignore;
        }

        public override DialogResult MessageBox(string msg, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defBtn)
        {
            var result = System.Windows.Forms.MessageBox.Show(msg, caption, buttons.ToMessageBoxButtons(), icon.ToMessageBoxIcon(),
                                                 defBtn.ToMessageBoxDefaultButton());
            return result.ToDialogResult();
        }

        //======================================================================================
        // screen output
        //======================================================================================
        public override void Write(string str)
        {
            foreach (Char ch in str)
            {
                if (ch == 7)
                {
                    Beep();
                }
                else
                    Console.Write(ch);
            }
        }
        public override void WriteLine()
        {
            Console.WriteLine();
        }
        public override void Beep()
        {
            if (GuineuInstance.Set.Bell)
            {
                if (GuineuInstance.Set.BellFile == null)
                    Console.Write((Char)7);
                else
                {
                    System.Media.SoundPlayer myPlayer = new System.Media.SoundPlayer();
                    myPlayer.SoundLocation = GuineuInstance.Set.BellFile;
                    myPlayer.Play();
                }
            }
        }

        public override int Wait(string text, int timeout, double X, double Y, bool window, bool clear)
        {
            if (timeout < 0)
                return 0;
            System.Windows.Forms.MessageBox.Show(text);
            return 0;
        }
    }
    class GUIWindow : Window
    {
        String windowName;
        Form linkedToForm;

        public GUIWindow(Form theForm)
        {
            this.linkedToForm = theForm;
        }

        public override void PutStr(string str)
        {
            Console.Write(str);
        }
        public String Name
        {
            get { return windowName; }
            set { windowName = value; }
        }
        public override Boolean Visible
        {
            get { return linkedToForm.Visible; }
        }
    }

    internal static class MessageBoxButtonsExtension
    {
        internal static System.Windows.Forms.MessageBoxButtons ToMessageBoxButtons(this MessageBoxButtons value)
        {
            switch (value)
            {
                case MessageBoxButtons.YesNo:
                    return System.Windows.Forms.MessageBoxButtons.YesNo;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
            ;
        }
    }

    internal static class MessageBoxIconExtension
    {
        internal static System.Windows.Forms.MessageBoxIcon ToMessageBoxIcon(this MessageBoxIcon value)
        {
            switch (value)
            {
                case MessageBoxIcon.Question:
                    return System.Windows.Forms.MessageBoxIcon.Question;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }
    }

    internal static class MessageBoxDefaultButtonExtension
    {
        internal static System.Windows.Forms.MessageBoxDefaultButton ToMessageBoxDefaultButton(this MessageBoxDefaultButton value)
        {
            switch (value)
            {
                case MessageBoxDefaultButton.Button1:
                    return System.Windows.Forms.MessageBoxDefaultButton.Button1;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }
    }

    internal static class DialogResultExtension
    {
        internal static DialogResult ToDialogResult(this System.Windows.Forms.DialogResult value)
        {
            switch (value)
            {
                case System.Windows.Forms.DialogResult.No:
                    return DialogResult.No;
                case System.Windows.Forms.DialogResult.Yes:
                    return DialogResult.Yes;
                case System.Windows.Forms.DialogResult.OK:
                    return DialogResult.OK;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }
    }

}
