

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using static System.Console;
using System.Web.Script.Serialization;
using System.Linq;

public class CalcGui : Form
{
  // add text box to form
  private TextBox TB(Font f, ref Point p, int width, int cg) {
    var tb = new TextBox();
    tb.Location = p;
    tb.Font = f;
    tb.Width = width;
    tb.ReadOnly = true;
    tb.TextAlign = HorizontalAlignment.Right;
    tb.Text = "01234567";
    this.Controls.Add(tb);
    p = p + new Size(0,tb.Height+cg);
    return tb;
  }

  /*-------------------------------------------------------------------------
  Script the button layout. This is much easier (and more reliable) to change
  than a bunch of mousing around in form editor. Map ascii art into a list of
  structs.  This is used to dynamically create buttons.
  */

  private struct ButtonLayout {
    public int row;      // 0,1,...
    public int col;      // 0,1,...
    public string text;  // eg:  "+" or "bs"
    public int width;    // 1,2,...
  };

  private string ButtonLayoutString = @"
    +---+---+---+---+---+
    | 1 | 2 | 3 | + | C |
    +---+---+---+---+---+
    | 4 | 5 | 6 | - | bs|
    +---+---+---+---+---+
    | 7 | 8 | 9 | * | % |
    +---+---+---+---+---+
    |   0   | . | / | = |
    +-------+---+---+---+
  ";

  private List<ButtonLayout> GetButtonLayout() {
    // get array of lines
    var a1 = ButtonLayoutString.Split('\n');
    // remove leading and trailing spaces in each line
    var a2 = new List<string>();
    foreach (string l in a1) {
      var l1 = l.Trim();
      a2.Add(l1);
    }
    // remove unneeded lines
    var a3 = new List<string>();
    foreach (string l in a2) {
      if ( ("" != l) && ('+' != l[0]) ) {
        a3.Add(l);
      }
    }
    // get text items
    var js = new JavaScriptSerializer();
    var t = new List<List<string>>();
    foreach (string l in a3) {
      var l1 = l.Replace("|","");
      var l2 = l1.Split(" ",StringSplitOptions.RemoveEmptyEntries);
      var l3 = new List<string>(l2);
      t.Add(l3);
    }
    //WriteLine(js.Serialize(t));

    // get widths
    var w = new List<List<int>>();
    foreach (string l in a3) {
      var l1 = l.Split("|",StringSplitOptions.RemoveEmptyEntries);
      var w1 = new List<int>();
      foreach(string l2 in l1) {
        w1.Add(l2.Length);
      }
      w.Add(w1);
    }
    // assumes that there is one narrow button in first row
    int cell_width = w[0].Min();

    // convert width to multiples of smallest cell
    var w2 = new List<List<int>>();
    foreach (List<int> l in w) {
      var w3 = new List<int>();
      foreach(int i in l) {
        w3.Add(i/cell_width);
      }
      w2.Add(w3);
    }
    //WriteLine(js.Serialize(w2));

    // combine text and widths to get final layout
    var lbl = new List<ButtonLayout>();
    for (int i=0; i<t.Count; i++) {
      var tr = t[i];
      var wr = w2[i];
      int k=0;
      for (int j=0; j<tr.Count; j++) {
        ButtonLayout bl;
        bl.row = i;
        bl.col = k;
        bl.text = tr[j];
        bl.width = wr[j];
        lbl.Add(bl);
        k += wr[j];
      }
    }
    //WriteLine(js.Serialize(lbl));
    return lbl;
  }

  /*------------------------------------------------------------------------
  Initialize the window
  */

  public CalcGui() {

    Text = "P1 Calculator";

    // form spacing
    int bs = 80;      // button size
    int br = 4;       // button rows
    int bc = 5;       // button columns
    int cg = 20;      // gap between controls
    int wbf = 4;      // window border fudge

    // window width
    int wx = bc*bs + (bc+1)*cg;
    // text box width
    int tbx = wx-2*cg;

    // size of 1-wide and 2-wide buttons
    var bs1 = new Size(bs,bs);
    var bs2 = new Size(bs*2+cg,bs);

    var font_btn = new Font("Microsoft Sans Serif", 20);
    var font_in  = new Font("Microsoft Sans Serif", 12);
    var font_out = new Font("Microsoft Sans Serif", 20);

    // starting location to add controls
    var loc = new Point(cg-wbf,cg);

    // input and output text boxes
    var ti = TB(font_in,  ref loc, tbx, cg);
    var to = TB(font_out, ref loc, tbx, cg);

    // window height, now can set size
    int wy = br*bs + (br+1)*cg + loc.Y;
    Size = new Size(wx,wy);

    // keep window from being resized
    this.MinimumSize = this.Size;
    this.MaximumSize = this.Size;
    this.MaximizeBox = false;

    // instantiate all the buttons
    var lbl = GetButtonLayout();
    foreach(ButtonLayout bl in lbl) {
      var b = new Button();
      b.Text = bl.text;
      int x = bl.col*(bs+cg);
      int y = bl.row*(bs+cg);
      b.Location = loc + new Size(x,y);
      int w = bl.width*bs + (bl.width-1)*cg;
      b.Size = new Size(w,bs);
      b.Font = font_btn;
      b.Click += ButtonClick;
      b.Visible = true;
      this.Controls.Add(b);
    }

    // logic handler
    var logic = new Logic();
  }

  private void ButtonClick(object sender, EventArgs e) {
    var s = (sender as Button).Text;
    WriteLine($"clicked {s}");
    var ok = logic.DoKey(s);
    var m = logic.GetError;
    var i = logic.GetInputs;
    var o = logic.Eval;
    ti.Text = i;
    to.Text = o;
  }

  static public void Main() {
    Application.Run(new CalcGui());
  }
}
