// Calculator logic -- accept keys, maintain internal state

using System;
using static System.Console;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;


class Logic {

  // member data
  private string Inputs;
  private string EqualAtEnd;
  private string Error;

  // initializer
  public Logic() {
    this.Inputs = "";
    this.EqualAtEnd = "";
    this.Error = "";
  }

  // get current inputs
  public string GetInputs() {
    return this.Inputs + this.EqualAtEnd;
  }

  // get error message
  public string GetError() {
    return this.Error;
  }

  // handle input key as string, convert to character
  public bool DoKey(string ks) {
    // allow string aliases for keys
    var KeyMap = new Dictionary<string,char> {
      { "Backspace" , '\b'},
    };
    // try to map it
    if ( KeyMap.ContainsKey(ks) )  return DoKey(KeyMap[ks]);
    // if single character, use it
    if ( 1 == ks.Length )          return DoKey(ks[0]);
    // error unrecognized key string
    this.Error = $"Unexpected keystring: {safe_fmt(ks)}";
    return false;
  }

 /*-----------------------------------------------------
  Handle input key as character
 */

  private bool DoKey(char kcr) {
    // default to no error
    this.Error = "";
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
    if ( ! KeySet.Contains(kc) ) {
      this.Error = $"Unexpected key: {safe_fmt(kcr)}";
      return false;
    }

    // handle deferred clear
    if ( "=" == this.EqualAtEnd) {
      this.Inputs = "";
      this.EqualAtEnd = "";
    }

    // backspace - remove last character of input
    var last = this.Inputs.Length -1;
    if ('\b' == kc) {
      if (last >= 0) this.Inputs = this.Inputs.Remove(last,1);
      return true;
    }

    // clear
    if ('c' == kc) {
      this.Inputs = "";
      return true;
    }

    // "="
    if ('=' == kc) {
      this.EqualAtEnd = "=";
      return true;
    }

    // digits and operators -- check if ok to accept in context
    if ( KeyOk(kc) ) {
      this.Inputs += kc;
      return true;
    }

    // report helpful message about why key was rejected
    this.Error = WhatKeysOk();
    return false;
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
  safe formatting for messages
  avoid putting control characters into messages
  */

  public string safe_fmt(char c) {
    var ki = (int)c;
    string f;
    if (Char.IsControl(c)) f = $"0x{ki:X2}";
    else                   f = $"'{c}'";
    return f;
  }

  public string safe_fmt(string s) {
    string f;
    if (1 == s.Length)  f = $"[{safe_fmt(s[0])}]";
    else                f = $"\"{s}\"";
    return f;
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
