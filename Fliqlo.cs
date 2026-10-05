using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fliqlo.Properties;
using Microsoft.Win32;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyTitle("Fliqlo")]
[assembly: AssemblyDescription("Flip clock screensaver")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("9031")]
[assembly: AssemblyProduct("Fliqlo")]
[assembly: AssemblyCopyright("© 2021 9031")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: Guid("7d5fd922-4226-42d3-8e7a-3891bd75cb17")]
[assembly: AssemblyFileVersion("1.5.1.0")]
[assembly: AssemblyInformationalVersion("1.5.1")]
[assembly: TargetFramework(".NETFramework,Version=v4.5.2", FrameworkDisplayName = ".NET Framework 4.5.2")]
[assembly: AssemblyVersion("1.5.1.0")]
namespace Fliqlo
{
	public class SaverForm : Form
	{
		private enum SaverWindowType
		{
			Saver,
			Preview,
			Test
		}

		[ComVisible(true)]
		public class JsClasss
		{
			public void FontError()
			{
				new ToolTip().SetToolTip(pictureBox3, "Missing Font");
				pictureBox3.Visible = true;
			}

			public void FontOK()
			{
				pictureBox3.Parent.Controls.Remove(pictureBox3);
				pictureBox3.Dispose();
			}
		}

		private RegistryKey regKey;

		private string appName;

		private SaverWindowType myWindowType;

		private int myScreenIndex;

		private Rectangle myBounds;

		private int oldX;

		private int oldY;

		private const string strUserAgent = "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko Fliqlo Screensaver";

		private const string strURL = "https://fliqlo.app/?c=flql&v=1.5.1&b=1";

		private PictureBox pictureBox1;

		public static PictureBox pictureBox3;

		private WebBrowser webBrowser1;

		private bool isOK;

		private static float DpiScale = new Form().CreateGraphics().DpiX / 96f;

		private bool WebBrowserDocumentEventSet;

		private IContainer components;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				if (myWindowType == SaverWindowType.Saver)
				{
					createParams.ExStyle |= 128;
				}
				if (myWindowType == SaverWindowType.Preview)
				{
					createParams.Style |= 1073741824;
				}
				return createParams;
			}
		}

		private void SetRegKey()
		{
			appName = Process.GetCurrentProcess().MainModule.FileName;
			appName = appName.Substring(appName.LastIndexOf('\\') + 1, appName.Length - appName.LastIndexOf('\\') - 1);
			if (appName.Contains("vhost"))
			{
				appName = appName.Substring(0, appName.IndexOf('.') + 1) + "exe";
			}
			try
			{
				regKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION");
				regKey.SetValue(appName, 11001, RegistryValueKind.DWord);
			}
			catch
			{
			}
		}

		private void DeleteRegKey()
		{
			try
			{
				regKey.DeleteValue(appName);
				regKey.Close();
			}
			catch
			{
			}
		}

		public SaverForm(Screen saverTargetScreen, int screenIndex)
		{
			InitializeComponent();
			SetRegKey();
			myWindowType = SaverWindowType.Saver;
			myScreenIndex = screenIndex;
			SuspendLayout();
			BackColor = Color.Black;
			FormBorderStyle = FormBorderStyle.None;
			WindowState = FormWindowState.Normal;
			StartPosition = FormStartPosition.Manual;
			ShowInTaskbar = false;
			TopMost = true;
			Location = saverTargetScreen.Bounds.Location;
			ClientSize = saverTargetScreen.Bounds.Size;
			if (Screen.AllScreens.Length < 2)
			{
				LostFocus += SaverForm_LostFocus;
			}
			else
			{
				myBounds = saverTargetScreen.Bounds;
			}
			ResumeLayout(performLayout: false);
		}

		public SaverForm()
		{
			InitializeComponent();
			SetRegKey();
			myWindowType = SaverWindowType.Preview;
			myScreenIndex = 0;
			SuspendLayout();
			BackColor = Color.Black;
			FormBorderStyle = FormBorderStyle.None;
			WindowState = FormWindowState.Maximized;
			TopMost = false;
			ResumeLayout(performLayout: false);
			Size clientSize = ClientSize;
			ClientSize = clientSize;
			Location = new Point(0, 0);
		}

		public SaverForm(int width, int height)
		{
			InitializeComponent();
			SetRegKey();
			myWindowType = SaverWindowType.Test;
			myScreenIndex = 0;
			SuspendLayout();
			ClientSize = new Size(width, height);
			TopMost = false;
			MinimizeBox = true;
			MaximizeBox = false;
			ControlBox = true;
			ShowIcon = true;
			ShowInTaskbar = true;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			StartPosition = FormStartPosition.WindowsDefaultLocation;
			WindowState = FormWindowState.Normal;
			ResumeLayout(performLayout: false);
		}

		private void SaverForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			DeleteRegKey();
		}

		private void SaverForm_Load(object sender, EventArgs e)
		{
			InitializeScreenSaver();
		}

		[DebuggerStepThrough]
		private void SaverForm_Shown(object sender, EventArgs e)
		{
			if (myWindowType == SaverWindowType.Saver)
			{
				int windowThreadProcessId = NativeMethods.GetWindowThreadProcessId(NativeMethods.GetForegroundWindow(), IntPtr.Zero);
				int windowThreadProcessId2 = NativeMethods.GetWindowThreadProcessId(Handle, IntPtr.Zero);
				if (windowThreadProcessId != windowThreadProcessId2)
				{
					NativeMethods.AttachThreadInput(windowThreadProcessId2, windowThreadProcessId, fAttach: true);
					Activate();
					NativeMethods.AttachThreadInput(windowThreadProcessId2, windowThreadProcessId, fAttach: false);
				}
			}
		}

		[DebuggerStepThrough]
		private void SaverForm_LostFocus(object sender, EventArgs e)
		{
			if (myWindowType == SaverWindowType.Saver)
			{
				int windowThreadProcessId = NativeMethods.GetWindowThreadProcessId(NativeMethods.GetForegroundWindow(), IntPtr.Zero);
				int windowThreadProcessId2 = NativeMethods.GetWindowThreadProcessId(Handle, IntPtr.Zero);
				if (windowThreadProcessId == windowThreadProcessId2)
				{
					Close();
				}
			}
		}

		[DebuggerStepThrough]
		private void SaverForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (myWindowType == SaverWindowType.Saver)
			{
				Close();
			}
		}

		[DebuggerStepThrough]
		private void SaverForm_MouseDown(object sender, MouseEventArgs e)
		{
			if (myWindowType == SaverWindowType.Saver)
			{
				Close();
			}
		}

		[DebuggerStepThrough]
		private void SaverForm_MouseMove(object sender, MouseEventArgs e)
		{
			if (myWindowType == SaverWindowType.Saver)
			{
				if ((oldX > 0) & (oldY > 0) & ((Math.Abs(e.X - oldX) > 1) | (Math.Abs(e.Y - oldY) > 1)))
				{
					Close();
				}
				oldX = e.X;
				oldY = e.Y;
			}
		}

		[DebuggerStepThrough]
		private void SaverForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (myWindowType == SaverWindowType.Saver)
			{
				Application.ExitThread();
			}
		}

		private void InitializeScreenSaver()
		{
			if (myWindowType == SaverWindowType.Saver && Screen.AllScreens.Length >= 2)
			{
				Bounds = myBounds;
			}
			LocalServer.EnsureStarted();
			bool isNetworkAvailable = true;
			isOK = true;
			SuspendLayout();
			pictureBox1 = new PictureBox
			{
				Size = Size,
				BackColor = Color.Black,
				Dock = DockStyle.Fill
			};
			pictureBox1.Enabled = false;
			int num = ((myWindowType == SaverWindowType.Saver) ? 64 : 16);
			pictureBox3 = new PictureBox();
			pictureBox3.Location = new Point((int)((float)(pictureBox1.Width / 2) - (float)(num / 2) * DpiScale), (int)((float)(pictureBox1.Height / 2) - (float)(num / 2) * DpiScale));
			pictureBox3.Size = new Size((int)((float)num * DpiScale), (int)((float)num * DpiScale));
			pictureBox3.BackColor = Color.Black;
			pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
			pictureBox3.Image = Resources.error;
			Controls.Add(pictureBox3);
			if (isOK)
			{
				pictureBox3.Visible = false;
				webBrowser1 = new WebBrowser();
				webBrowser1.ObjectForScripting = new JsClasss();
				webBrowser1.Size = Size;
				webBrowser1.Visible = false;
				webBrowser1.AllowNavigation = false;
				webBrowser1.DocumentCompleted += WebBrowser1_DocumentCompleted;
				webBrowser1.Dock = DockStyle.Fill;
				((Control)webBrowser1).Enabled = false;
				Controls.Add(pictureBox1);
				Controls.Add(webBrowser1);
				ResumeLayout();
				webBrowser1.ScriptErrorsSuppressed = true;
				webBrowser1.ScrollBarsEnabled = false;
				webBrowser1.Navigate(LocalServer.GetUrl("c=flql&v=1.5.1&b=1"), "_self", null, "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko Fliqlo Screensaver");
			}
			else
			{
				new ToolTip().SetToolTip(pictureBox3, (!isNetworkAvailable) ? "No Internet Connection" : "Registry Error");
				pictureBox3.Visible = true;
				Controls.Add(pictureBox1);
				ResumeLayout();
			}
		}

		private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			SuspendLayout();
			WebBrowser webBrowser = sender as WebBrowser;
			if (webBrowser.Document.Url.ToString().StartsWith("res://ieframe.dll"))
			{
				new ToolTip().SetToolTip(pictureBox3, "Content Not Found");
				pictureBox3.Visible = true;
			}
			else if (webBrowser.ReadyState == WebBrowserReadyState.Complete && !WebBrowserDocumentEventSet)
			{
				WebBrowserDocumentEventSet = true;
				webBrowser1.Visible = true;
				pictureBox1.Visible = false;
				Controls.Remove(pictureBox1);
				pictureBox1.Dispose();
			}
			ResumeLayout();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(192f, 192f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			base.ClientSize = new System.Drawing.Size(751, 459);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Margin = new System.Windows.Forms.Padding(4);
			base.MaximizeBox = false;
			base.Name = "SaverForm";
			this.Text = "Screen Saver";
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SaverForm_FormClosing);
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SaverForm_FormClosed);
			base.Load += new System.EventHandler(this.SaverForm_Load);
			base.Shown += new System.EventHandler(this.SaverForm_Shown);
			base.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SaverForm_KeyDown);
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SaverForm_MouseDown);
			base.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SaverForm_MouseMove);
			base.ResumeLayout(false);
		}
	}
	public class ConfigForm : Form
	{
		[ComVisible(true)]
		public class JsClasss
		{
			public void FontError()
			{
				new ToolTip().SetToolTip(pictureBox3, "Missing Font");
				pictureBox3.Visible = true;
			}

			public void FontOK()
			{
				pictureBox3.Parent.Controls.Remove(pictureBox3);
				pictureBox3.Dispose();
			}

			public void ConsoleLog(string message)
			{
				Console.WriteLine("console.log:" + message);
			}

			public void FocusIn()
			{
				if (Form.ActiveForm != null)
				{
					Form.ActiveForm.AcceptButton = null;
				}
			}

			public void FocusOut()
			{
				Form activeForm = Form.ActiveForm;
				if (activeForm != null)
				{
					activeForm.AcceptButton = button1;
				}
			}
		}

		private int wmTaskbarButtunCreated;

		private RegistryKey regKey;

		private string appName;

		public const int WM_NCLBUTTONDOWN = 161;

		private const string strUserAgent = "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko Fliqlo Screensaver";

		private const string strURL = "https://fliqlo.app/?c=flql&v=1.5.1&b=1&mode=set";

		private LnkLabel linkLabel1;

		private ToolTip toolTip1;

		public static CustomButton button1;

		private Bitmap bitmap;

		private PictureBox pictureBox1;

		private PictureBox pictureBox2;

		public static PictureBox pictureBox3;

		private WebBrowser webBrowser1;

		private bool isOK;

		private static float DpiScale = new Form().CreateGraphics().DpiX / 96f;

		private bool WebBrowserDocumentEventSet;

		private IContainer components;

		private void SetRegKey()
		{
			appName = Process.GetCurrentProcess().MainModule.FileName;
			appName = appName.Substring(appName.LastIndexOf('\\') + 1, appName.Length - appName.LastIndexOf('\\') - 1);
			if (appName.Contains("vhost"))
			{
				appName = appName.Substring(0, appName.IndexOf('.') + 1) + "exe";
			}
			try
			{
				regKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION");
				regKey.SetValue(appName, 11001, RegistryValueKind.DWord);
			}
			catch
			{
			}
		}

		private void DeleteRegKey()
		{
			try
			{
				regKey.DeleteValue(appName);
				regKey.Close();
			}
			catch
			{
			}
		}

		public ConfigForm()
		{
			InitializeComponent();
			wmTaskbarButtunCreated = NativeMethods.RegisterWindowMessage("TaskbarButtonCreated");
			SetRegKey();
			BackColor = ColorTranslator.FromHtml("#252526");
			Paint += ConfigForm_Paint;
			Shown += ConfigForm_Shown;
			InitializeConfigDialog();
		}

		private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			DeleteRegKey();
		}

		[DebuggerStepThrough]
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == wmTaskbarButtunCreated)
			{
				ShowIcon = false;
			}
			else if (m.Msg == 161 && isOK && webBrowser1.ReadyState == WebBrowserReadyState.Complete)
			{
				webBrowser1.Document.InvokeScript("hideDevSetting");
			}
			base.WndProc(ref m);
		}

		private void ConfigForm_Load(object sender, EventArgs e)
		{
			MouseDown += ConfigForm_MouseDown;
		}

		private void InitializeConfigDialog()
		{
			using (Form form = new Form())
			{
				WebBrowser webBrowser = new WebBrowser();
				form.Controls.Add(webBrowser);
				webBrowser.CreateControl();
				form.Close();
			}
			LocalServer.EnsureStarted();
			bool isNetworkAvailable = true;
			isOK = true;
			SuspendLayout();
			ClientSize = new Size((int)(480f * DpiScale), (int)(320f * DpiScale));
			Text = "Fliqlo Settings";
			pictureBox1 = new PictureBox
			{
				Location = new Point(0, 0),
				Size = new Size((int)(480f * DpiScale), (int)(270f * DpiScale)),
				BackColor = Color.Black,
				SizeMode = PictureBoxSizeMode.CenterImage
			};
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
			string productVersion = versionInfo.ProductVersion;
			string legalCopyright = versionInfo.LegalCopyright;
			linkLabel1 = new LnkLabel();
			linkLabel1.Parent = this;
			linkLabel1.Name = "Label1";
			linkLabel1.Text = productVersion + " " + legalCopyright;
			linkLabel1.Font = new Font("Arial", 9f);
			linkLabel1.AutoSize = true;
			linkLabel1.BackColor = Color.Transparent;
			linkLabel1.DisabledLinkColor = Color.Black;
			linkLabel1.VisitedLinkColor = Color.Black;
			linkLabel1.LinkBehavior = LinkBehavior.NeverUnderline;
			linkLabel1.LinkColor = ColorTranslator.FromHtml("#888888");
			linkLabel1.ActiveLinkColor = ColorTranslator.FromHtml("#cccccc");
			linkLabel1.Location = new Point((int)(10f * DpiScale), (int)(287f * DpiScale));
			linkLabel1.TabStop = false;
			linkLabel1.Click += LinkLabel1Clicked;
			linkLabel1.MouseDown += ConfigForm_MouseDown;
			linkLabel1.MouseEnter += LinkLabel1_MouseEnter;
			linkLabel1.MouseLeave += LinkLabel1_MouseLeave;
			toolTip1 = new ToolTip();
			toolTip1.SetToolTip(linkLabel1, "Open site in browser");
			button1 = new CustomButton();
			button1.Parent = this;
			button1.FlatStyle = FlatStyle.Flat;
			button1.BackColor = ColorTranslator.FromHtml("#2d2d30");
			button1.FlatAppearance.BorderSize = 0;
			button1.FlatAppearance.MouseDownBackColor = ColorTranslator.FromHtml("#3f3f46");
			button1.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#333337");
			button1.Name = "Button1";
			button1.Text = "OK";
			button1.Font = new Font("Arial", 9f);
			button1.ForeColor = Color.LightGray;
			button1.Location = new Point((int)(390f * DpiScale), (int)(280f * DpiScale));
			button1.Size = new Size((int)(80f * DpiScale), (int)(30f * DpiScale));
			button1.Margin = new Padding(0);
			button1.Padding = new Padding(0, 0, 0, 0);
			button1.AutoSize = true;
			AcceptButton = button1;
			button1.Click += Button1Clicked;
			button1.TabIndex = 1;
			button1.MouseDown += ConfigForm_MouseDown;
			Controls.Add(linkLabel1);
			Controls.Add(button1);
			pictureBox3 = new PictureBox();
			pictureBox3.Location = new Point((int)(224f * DpiScale), (int)(119f * DpiScale));
			pictureBox3.Size = new Size((int)(32f * DpiScale), (int)(32f * DpiScale));
			pictureBox3.BackColor = Color.Black;
			pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
			pictureBox3.Image = Resources.error;
			Controls.Add(pictureBox3);
			if (isOK)
			{
				pictureBox3.Visible = false;
				bitmap = Resources.spinner;
				pictureBox2 = new PictureBox();
				pictureBox2.Location = new Point((int)(232f * DpiScale), (int)(127f * DpiScale));
				pictureBox2.Size = new Size((int)(16f * DpiScale), (int)(16f * DpiScale));
				pictureBox2.BackColor = Color.Black;
				pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
				pictureBox1.Enabled = false;
				webBrowser1 = new WebBrowser();
				webBrowser1.ObjectForScripting = new JsClasss();
				webBrowser1.Location = new Point(0, 0);
				webBrowser1.TabStop = false;
				webBrowser1.Size = new Size((int)(480f * DpiScale), (int)(270f * DpiScale));
				webBrowser1.Visible = false;
				webBrowser1.AllowNavigation = false;
				((Control)webBrowser1).Enabled = false;
				webBrowser1.DocumentCompleted += WebBrowser1_DocumentCompleted;
				Controls.Add(pictureBox2);
				Controls.Add(pictureBox1);
				ResumeLayout();
				webBrowser1.ScriptErrorsSuppressed = true;
				webBrowser1.ScrollBarsEnabled = false;
				webBrowser1.Navigate(LocalServer.GetUrl("c=flql&v=1.5.1&b=1&mode=set&zoom=" + DpiScale), "_self", null, "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko Fliqlo Screensaver");
			}
			else
			{
				new ToolTip().SetToolTip(pictureBox3, (!isNetworkAvailable) ? "No Internet Connection" : "Registry Error");
				pictureBox3.Visible = true;
				Controls.Add(pictureBox1);
				ResumeLayout();
			}
			Invalidate();
			PerformLayout();
		}

		private void LinkLabel1_MouseEnter(object sender, EventArgs e)
		{
			((LinkLabel)sender).LinkColor = ColorTranslator.FromHtml("#aaaaaa");
		}

		private void LinkLabel1_MouseLeave(object sender, EventArgs e)
		{
			((LinkLabel)sender).LinkColor = ColorTranslator.FromHtml("#888888");
		}

		private void ConfigForm_Paint(object sender, PaintEventArgs e)
		{
			using Graphics graphics = e.Graphics;
			graphics.FillRectangle(Brushes.Black, 0f, 0f, 480f * DpiScale, 270f * DpiScale);
		}

		private void ConfigForm_MouseDown(object sender, MouseEventArgs e)
		{
			if (isOK && webBrowser1.ReadyState == WebBrowserReadyState.Complete)
			{
				webBrowser1.Document.InvokeScript("hideDevSetting");
			}
		}

		private void Image_FrameChanged(object o, EventArgs e)
		{
			pictureBox2.Invalidate();
		}

		private void PictureBox0_Paint(object sender, PaintEventArgs e)
		{
			ImageAnimator.UpdateFrames(bitmap);
		}

		private async void ConfigForm_Shown(object sender, EventArgs e)
		{
			if (isOK)
			{
				await Task.Delay(10);
				pictureBox2.Image = bitmap;
				pictureBox2.Paint += PictureBox0_Paint;
				ImageAnimator.Animate(bitmap, Image_FrameChanged);
			}
		}

		private void LinkLabel1Clicked(object sender, EventArgs e)
		{
			Process.Start("https://fliqlo.com/");
		}

		private void Button1Clicked(object sender, EventArgs e)
		{
			Close();
		}

		private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			SuspendLayout();
			WebBrowser webBrowser = sender as WebBrowser;
			if (webBrowser.Document.Url.ToString().StartsWith("res://ieframe.dll"))
			{
				new ToolTip().SetToolTip(pictureBox3, "Content Not Found");
				pictureBox2.Visible = false;
				pictureBox3.Visible = true;
				Controls.Remove(pictureBox2);
				pictureBox2.Dispose();
			}
			else if (webBrowser.ReadyState == WebBrowserReadyState.Complete && !WebBrowserDocumentEventSet)
			{
				WebBrowserDocumentEventSet = true;
				webBrowser.Document.Body.MouseLeave += OnHtmlDocumentLeave;
				webBrowser1.ScrollBarsEnabled = false;
				webBrowser1.ScriptErrorsSuppressed = true;
				webBrowser1.Visible = true;
				Controls.Add(webBrowser1);
				((Control)webBrowser1).Enabled = true;
				pictureBox2.Visible = false;
				pictureBox1.Visible = false;
				Controls.Remove(pictureBox2);
				pictureBox2.Dispose();
				Controls.Remove(pictureBox1);
				pictureBox1.Dispose();
			}
			ResumeLayout();
		}

		private void OnHtmlDocumentLeave(object sender, HtmlElementEventArgs e)
		{
			webBrowser1.Document.InvokeScript("leaveDevSetting");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fliqlo.ConfigForm));
			base.SuspendLayout();
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ConfigForm";
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigForm_FormClosing);
			base.Load += new System.EventHandler(this.ConfigForm_Load);
			base.ResumeLayout(false);
		}
	}
	public class CustomButton : Button
	{
		public CustomButton()
		{
			FlatAppearance.BorderSize = 0;
			FlatStyle = FlatStyle.Flat;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Pen pen = new Pen(ColorTranslator.FromHtml("#46464a"), 1f);
			Rectangle rect = new Rectangle(0, 0, Size.Width - 1, Size.Height - 1);
			e.Graphics.DrawRectangle(pen, rect);
		}
	}
	public class LnkLabel : LinkLabel
	{
		private const int WM_SETCURSOR = 32;

		private const int IDC_HAND = 32649;

		[DllImport("user32.dll")]
		public static extern int LoadCursor(int hInstance, int lpCursorName);

		[DllImport("user32.dll")]
		public static extern int SetCursor(int hCursor);

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 32)
			{
				SetCursor(LoadCursor(0, 32649));
				m.Result = IntPtr.Zero;
			}
			else
			{
				base.WndProc(ref m);
			}
		}
	}
	internal static class Program
	{
		private enum StartType
		{
			None,
			Config,
			Save,
			Preview,
			Test
		}

		[DebuggerNonUserCode]
		private sealed class NativeForm : IWin32Window
		{
			private readonly IntPtr handle;

			public IntPtr Handle => handle;

			public NativeForm(IntPtr hwnd)
			{
				handle = hwnd;
			}
		}

		[STAThread]
		[DebuggerNonUserCode]
		private static void Main(string[] cmdArgs)
		{
			if (Environment.OSVersion.Version.Major >= 6)
			{
				SetProcessDPIAware();
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
			StartType startType = StartType.None;
			IntPtr intPtr = IntPtr.Zero;
			for (int i = 0; i < cmdArgs.Length; i++)
			{
				string text = cmdArgs[i];
				if (startType == StartType.None)
				{
					text = text.ToLower();
					text = text.Replace("-", "/");
					int num = text.IndexOf("/");
					if (num >= 0)
					{
						text = text.Remove(0, num);
						if (text.StartsWith("/c"))
						{
							startType = StartType.Config;
						}
						else
						{
							if (!text.StartsWith("/p"))
							{
								startType = ((!text.StartsWith("/t")) ? StartType.Save : StartType.Test);
								break;
							}
							startType = StartType.Preview;
						}
					}
				}
				if (startType == StartType.None)
				{
					continue;
				}
				Regex regex = new Regex("(?<s>\\d+)", RegexOptions.Compiled);
				if (regex.Match(text).Success)
				{
					if (int.TryParse(regex.Match(text).Result("${s}"), out var result))
					{
						intPtr = new IntPtr(result);
						break;
					}
					return;
				}
			}
			if (startType == StartType.None)
			{
				startType = StartType.Config;
			}
			switch (startType)
			{
			case StartType.Config:
			{
				IntPtr intPtr2 = IntPtr.Zero;
				if (intPtr != IntPtr.Zero)
				{
					if (!NativeMethods.IsWindow(intPtr))
					{
						break;
					}
					intPtr2 = NativeMethods.GetAncestor(intPtr, 2);
				}
				ConfigForm configForm = new ConfigForm();
				if (intPtr2 == IntPtr.Zero)
				{
					configForm.SuspendLayout();
					configForm.FormBorderStyle = FormBorderStyle.FixedSingle;
					configForm.ShowInTaskbar = true;
					configForm.ShowIcon = true;
					configForm.StartPosition = FormStartPosition.WindowsDefaultLocation;
					configForm.ResumeLayout(performLayout: false);
					configForm.ShowDialog();
				}
				else
				{
					configForm.SuspendLayout();
					configForm.FormBorderStyle = FormBorderStyle.FixedSingle;
					configForm.ShowInTaskbar = false;
					configForm.ShowIcon = false;
					configForm.StartPosition = FormStartPosition.CenterParent;
					configForm.ResumeLayout(performLayout: false);
					configForm.ShowDialog(new NativeForm(intPtr2));
				}
				configForm.Dispose();
				break;
			}
			case StartType.Save:
				if (Screen.AllScreens.Length != 0)
				{
					Cursor.Hide();
					int num2 = 0;
					Screen[] allScreens = Screen.AllScreens;
					for (int i = 0; i < allScreens.Length; i++)
					{
						new SaverForm(allScreens[i], num2++).Show();
					}
					Application.Run();
				}
				break;
			case StartType.Preview:
				if (NativeMethods.IsWindow(intPtr))
				{
					SaverForm saverForm = new SaverForm();
					if (NativeMethods.SetParent(saverForm.Handle, intPtr) != IntPtr.Zero)
					{
						Application.Run(saverForm);
					}
				}
				break;
			case StartType.Test:
				Application.Run(new SaverForm(600, 400));
				break;
			}
		}

		[DllImport("user32.dll")]
		private static extern bool SetProcessDPIAware();
	}
	internal static class NativeMethods
	{
		private delegate int TaskDialogCallback(IntPtr hwndParent, IntPtr hInstance, [MarshalAs(UnmanagedType.LPWStr)] string pszWindowTitle, [MarshalAs(UnmanagedType.LPWStr)] string pszMainInstruction, [MarshalAs(UnmanagedType.LPWStr)] string pszContent, int dwCommonButtons, int pszIcon, out int pnButton);

		public const int E_FAIL = -2147467259;

		public const int GA_ROOT = 2;

		public const int GWL_STYLE = -16;

		public const int GWL_EXSTYLE = -20;

		public const int TD_INFORMATION_ICON = 65533;

		public const int TDCBF_OK_BUTTON = 1;

		public const int WM_NULL = 0;

		public const int WS_CHILD = 1073741824;

		public const int WS_POPUP = int.MinValue;

		public const int WS_EX_TOOLWINDOW = 128;

		[DllImport("user32.dll")]
		public static extern int AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);

		[DllImport("user32.dll")]
		public static extern IntPtr GetAncestor(IntPtr hwnd, int gaFlags);

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int lpdwProcessId);

		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(IntPtr hwnd, IntPtr zeroPtr);

		[DllImport("user32.dll")]
		public static extern bool IsWindow(IntPtr hwnd);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegisterWindowMessageW", SetLastError = true)]
		public static extern int RegisterWindowMessage(string lpString);

		[DllImport("user32.dll")]
		public static extern IntPtr SetParent(IntPtr hwndChild, IntPtr hwndNewParent);

		public static int TaskDialog(IntPtr hwndParent, IntPtr hInstance, string pszWindowTitle, string pszMainInstruction, string pszContent, int dwCommonButtons, int pszIcon, out int pnButton)
		{
			IntPtr hModule = LoadLibrary("comctl32.dll");
			IntPtr procAddress = GetProcAddress(hModule, "TaskDialog");
			int result;
			if (procAddress == IntPtr.Zero)
			{
				pnButton = 0;
				result = -2147467259;
			}
			else
			{
				result = ((TaskDialogCallback)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(TaskDialogCallback)))(hwndParent, hInstance, pszWindowTitle, pszMainInstruction, pszContent, dwCommonButtons, pszIcon, out pnButton);
			}
			FreeLibrary(hModule);
			return result;
		}

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32", BestFitMapping = false, CharSet = CharSet.Ansi, SetLastError = true, ThrowOnUnmappableChar = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern IntPtr LoadLibrary(string lpFileName);
	}
}

namespace Fliqlo.Properties
{
    [CompilerGenerated]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
        public static Settings Default => defaultInstance;
    }
}
