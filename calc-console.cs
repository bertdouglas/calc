// Console driver for calculator logic

using System;
using static System.Console;

public class CalcConsole
{
  private static void cls() {
    // clear screen
    // https://en.wikipedia.org/wiki/ANSI_escape_code#CSI_sequences
    Write("\u001B[2J\u001B[H");
  }

  public static void Main(string[] args)
  {
    var logic = new Logic();
    logic.DoTests();

    var topline = "Calculator - Console test driver.  Control-C to exit;";
    int n_keys = 0;
    while (true) {
      // get a key
      char kc;
      string ke;
      if (0 == n_keys) {
        // fake the clear key one time at start
        // so display is not blank, while waiting for key
        kc = 'c';
        ke = "C";
      }
      else {
        // wait for key without any echo to console
        var cki = ReadKey(true);
        // two representations of key, as character, and as enum string repr
        kc = cki.KeyChar;
        ke = cki.Key.ToString();
      }
      n_keys += 1;

      /* There is some weirdness and inconsistency in the results from
      ReadKey(). cki.Key is an enum, but for a few important keys (like '=' and
      '.' on numeric keypad) this enum has duplicated value of zero, not
      allowing keys to be distinguished.  cki.KeyChar is a character, and for a
      few important keys (like backspace) it has the value of zero.  Through
      experiment, it seems best to prefer cki.KeyChar, unless it is zero.  Then
      to use cki.Key, unless it is zero. Some keys on numeric keypad (with
      num-lock off) are wrongly decoded. Fortunately we can just keep num-lock
      on.  This may be a bug in mono on Linux. */

      /* The goal here is to be a dumb pipe and to route keys as simply and
      directly as possible to the destination, without losing any information.
      */

      // failed to decode key, ignore
      if ( (0 == kc) && ("0" == ke) ) continue;

      // use heuristic to combine values
      var k = "";
      if ( 0 != kc) k = kc.ToString();
      else          k = ke;

      // process the key through logic and get results
      var p = logic.DoKey(k);
      var i = logic.GetInputs();
      var o = logic.Eval();
      var m = logic.GetError();

      // show results
      cls();
      WriteLine(topline);
      /*
      Used for debugging ReadKey()
      Write($"n_keys:{n_keys}");
      Write($"   kc:{logic.safe_fmt(kc)}");
      Write($"   ke:\"{ke}\"");
      Write($"   k:{logic.safe_fmt(k)}\n");
      */
      WriteLine($"i {i,40}");
      WriteLine($"o {o,40}");
      WriteLine($"m {m}");
    }
  }
}
