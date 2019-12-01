// Calculator logic -- accept keys, maintain internal state

using System;
using static System.Console;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

class Logic {

  // Status codes
  public enum SC {
    OK = 0,
    EE_USNUM = -100,  // error: expecting a number without sign
                      //   eg:  1  1.0  0.1  .1
    EE_OSNUM,         // error: expecting a number with optional sign
                      //   eg:  -1 +1 +1.0 -1.0 -0.1 +0.1 -.1 +.1
                      //   eg:  1 1.0 0.1 .1
    EE_FRAC,          // error: expecting fraction
                      //   eg:  1 01 001 0001 123450
    EX_DEC,           // error: extra decimal point
    EE_OPER,          // error: expecting operator
    IU_KEY = -200,    // internal error:  Unknown key press
  };

  // get status string from status code
  public string GetStatusString(SC sc) {
    var StsMsg = new Dictionary<SC, string> {
      { SC.OK,         ""                                                 },
      { SC.EE_USNUM,   "Error: expecting a number without sign."          },
      { SC.EE_OSNUM,   "Error: expecting a number with optional sign."    },
      { SC.EE_FRAC,    "Error: expecting fractional part of number."      },
      { SC.EX_DEC,     "Error: extra decimal point."                      },
      { SC.EE_OPER,    "Error: expecting operator."                       },
      { SC.IU_KEY,     "Internal error:  Unknown key press."              },
    };
    return StsMsg[sc];
  }

  // list of keys accepted
  private string Inputs;
  private string EqualAtEnd;

  // initializer
  public Logic() {
    this.Inputs = "";
    this.EqualAtEnd = "";
  }

  // get current inputs
  public string GetInputs() {
    return this.Inputs + this.EqualAtEnd;
  }

  // handle input key as string, convert to character
  public SC DoKey(string ks) {
    // allow string aliases for keys
    var KeyMap = new Dictionary<string,char> {
      { "Backspace" , '\b'},
    };
    // try to map it
    if ( KeyMap.ContainsKey(ks) )  return DoKey(KeyMap[ks]);
    // if single character, use it
    if ( 1 == ks.Length )          return DoKey(ks[0]);
    // error unrecognized key string
    return SC.IU_KEY;
  }

 /*-----------------------------------------------------
  Handle input key as character
 */

  private SC DoKey(char kcr) {
    // each key is represented by a character
    char[] KeyChars = {
      '0','1','2','3','4','5','6','7','8','9',    // digits
      '.',                                        // decimal point
      '+','-','*','/','%',                        // operators
      'c',                                        // clear
      '=',                                        // deferred clear
      '\b',                                       // backspace
    };
    var KeySet = new HashSet<char>(KeyChars);

    // normalize to lower case
    char kc = Char.ToLower(kcr);

    // reject if unknown key
    if ( ! KeySet.Contains(kc) ) return SC.IU_KEY;

    // handle deferred clear
    if ( "=" == this.EqualAtEnd) {
      this.Inputs = "";
      this.EqualAtEnd = "";
    }

    // backspace - remove last character of input
    var last = this.Inputs.Length -1;
    if ('\b' == kc) {
      WriteLine("bs1");
      if (last >= 0) this.Inputs = this.Inputs.Remove(last,1);
      WriteLine($"bs2: {this.Inputs}");
      return SC.OK;
    }

    // clear
    if ('c' == kc) {
      this.Inputs = "";
      return SC.OK;
    }

    // "="
    if ('=' == kc) {
      this.EqualAtEnd = "=";
      return SC.OK;
    }

    // digits and operators -- check if ok to accept in context
    if ( KeyOk(kc) ) {
      this.Inputs += kc;
      return SC.OK;
    }

    // report helpful message about why key was rejected
    return SC.OK;
  }

  /*-----------------------------------------------------
  Predicates for acceptable keys in context
  */
  private static string s_op = $@"[\+\-\*\/\%]";
  private static string s_sign = $@"[\+\-]";

  private static string s_ok_sign     = $@"(?<!(?:{s_op}|^){s_sign})(?<!\.)$";
  private static string s_ok_op       = $@"\d$";
  private static string s_ok_decimal  = $@"(?<!\.\d*)$";

  Regex r_ok_sign     = new Regex(s_ok_sign);
  Regex r_ok_op       = new Regex(s_ok_op);
  Regex r_ok_decimal  = new Regex(s_ok_decimal);

  private bool p(Regex r) {
    Match m = r.Match(this.Inputs);
    return m.Success;
  }

  /*-------------------------------------------------------------------------
  Check if it is ok to accept a key given the context of previous input. This
  handles only operators and operands.  It does not handle clear or backspace.
  */

  private bool KeyOk(char c) {
    // digits always accepted
    if ( ('0' <= c) && (c <= '9') ) return true;
    // sign (may be operator if not sign)
    if ("+-".Contains(c)) { if (p(r_ok_sign)) return true; }
    // operator
    if ("+-*/%".Contains(c)) return p(r_ok_op);
    // decimal
    if ('.' == c) return p(r_ok_decimal);
    // should not get here
    throw new ArgumentException("Unexpected key.");
  }

  private string WhatKeysOk() {
    var a = new List<string>();
    a.Add("digit");
    if (p(r_ok_sign))    a.Add("sign");
    if (p(r_ok_op))      a.Add("operator");
    if (p(r_ok_decimal)) a.Add("decimal");

    var sa = String.Join(" or ",a);
    var s = $"Expecting: {sa}.";
    return s;
  }

  /*-------------------------------------------------------------------------
  */

  public string Eval() {
    string res;
    try {
      var dt = new DataTable();
      res = dt.Compute(this.Inputs, null).ToString();
    }
    catch {
      res = "(incomplete expression)";
    }
    return res;
  }

  /*-------------------------------------------------------------------------
  Test key accept predicates
  */

  private void TestKeyOk() {

    int n_failed = 0;
    int n_tests = 0;
    WriteLine("Start testing key accept predicates...");

    void t(Regex r, string s, bool v) {
      var m = r.Match(s);
      var fail = (v != m.Success);
      if (fail) n_failed += 1;
      n_tests += 1;
      string pf_msg;
      if (fail)  pf_msg = "failed";
      else       pf_msg = "passed";
      WriteLine($"    {s,-10}  {v,-5}  {pf_msg}");
    }

    WriteLine("\nr_ok_sign");
    t(r_ok_sign, "*",     true);
    t(r_ok_sign, "1",     true);
    t(r_ok_sign, "1+",    true);
    t(r_ok_sign, "1*",    true);
    t(r_ok_sign, "1.0+1", true);
    t(r_ok_sign, "1.0*1", true);
    t(r_ok_sign, ".",     false);
    t(r_ok_sign, "+-",    false);
    t(r_ok_sign, "*+",    false);
    t(r_ok_sign, "+",     false);

    WriteLine("\nr_ok_op");
    t(r_ok_op, "0",       true);
    t(r_ok_op, "1",       true);
    t(r_ok_op, "",        false);
    t(r_ok_op, "-",       false);
    t(r_ok_op, "+",       false);
    t(r_ok_op, "*",       false);
    t(r_ok_op, "/",       false);
    t(r_ok_op, "%",       false);
    t(r_ok_op, ".",       false);

    WriteLine("\nr_ok_decimal");
    t(r_ok_decimal, "",      true);
    t(r_ok_decimal, "%",     true);
    t(r_ok_decimal, "3.0*",  true);
    t(r_ok_decimal, "10.0",  false);
    t(r_ok_decimal, ".",     false);

    WriteLine("End Testing key accept predicates.");
    WriteLine($"{n_tests} tests.  {n_failed} failed.\n");
  }

  /*-------------------------------------------------------------------------
  Top level test of DoKey
  */

  private void TestDoKey() {
  }

  /*-------------------------------------------------------------------------
  Top level test wrapper
  */

  public void DoTests() {
    TestKeyOk();
    TestDoKey();
    ReadKey();
  }

} // Logic

//-------------------------------------------------------------------------
