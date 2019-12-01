
/*
string Button_Layout = @"
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
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using static System.Console;



public class CalcGui : Form
{
  public CalcGui()
  {
    Text = "Calculator";
    Size = new Size(400,400);

    var s1 = new Size(50,50);

    var font_btn = new Font("Microsoft Sans Serif", 20);
    var font_in  = new Font("Microsoft Sans Serif", 10);
    var font_out = new Font("Microsoft Sans Serif", 20);


    var ti = new TextBox();
    ti.Parent = this;
    ti.Location = new Point(0,0);
    ti.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
    ti.Size = new Size(400,40);
    ti.Text = "01234567";

    var buttons = new List<string>() {"0","1","2","C"};
    foreach (var btext in buttons) {
      int x = 0;
      int y = 40;
      var btn = new Button();
      btn.Text = btext;
      btn.Parent = this;
      btn.Location = new Point(x,y);
      x += 40;
      btn.Size = s1;
      btn.Font = font_btn;
      btn.Click += ButtonClick;
      btn.Visible = true;
      Controls.Add(btn);
      WriteLine(btext);
    }
  }

  private void ButtonClick(object sender, EventArgs e) {
  }

  static public void Main()
  {
    Application.Run(new CalcGui());
  }
}
