
// concise test wrapper
// arguments:
//   1: button press string from table above
//   2: bottom display line after button press
//   3: top (history) display line after button press
// action:
//   Apply button press, and check if display lines are correct


// example from pdf spec
  t("C",   "",             "");
  t("1",  "1",            "1");
  t("0", "10",           "10");
  t("+", "10",          "10+");
  t("1", "11",         "10+1");
  t("0", "20",        "10+10");
  t("+", "20",       "10+10+");
  t("1", "21",      "10+10+1");
  t("0", "30",     "10+10+10");
  t("/", "30",    "10+10+10/");
  t("3", "10",   "10+10+10/3");
  t("0",  "1",  "10+10+10/30");
  t("=",  "1", "10+10+10/30=");
  t("4",  "4",            "4");

// all digits
  t("C",           "",             "");
  t("1",          "1",            "1");
  t("2",         "12",           "12");
  t("3",        "123",          "123");
  t("4",       "1234",         "1234");
  t("5",      "12345",        "12345");
  t("6",     "123456",       "123456");
  t("7",    "1234567",      "1234567");
  t("8",   "12345678",     "12345678");
  t("9",  "123456789",    "123456789");
  t("0", "1234567890",   "1234567890");
  t("=", "1234567890",   "1234567890");
  t("5",          "5",            "5");

// leading zeroes
  t("C",           "",             "");
  t("0",          "0",            "0");
  t("0",          "0",            "0", WRN_EXTRA_ZERO);
  t("1",          "1",            "1");

// unary minus  (implicit multiply/divide by 1)
  t("C",           "",             "");
  t("-",          "0",            "-");
  t("2",         "-2",           "-2");
  t("*",         "-2",          "-2*");
  t("-",          "2",         "-2*-");
  t("3",          "6",        "-2*-3");
  t("/",          "6",       "-2*-3/");
  t("-",         "-6",      "-2*-3/-");
  t("2",         "-3",     "-2*-3/-2");
// with backspace
  t("bs",        "-6",      "-2*-3/-");
  t("bs",         "6",       "-2*-3/");
  t("bs",         "6",        "-2*-3");
  t("bs",         "2",         "-2*-");
  t("bs",        "-2",          "-2*");
  t("bs",        "-2",           "-2");
  t("bs",         "0",            "-");
  t("bs",          "",             "");


// mixed signs (implicit add/sub by 0)
  t("C",           "",             "");
  t("-",          "0",            "-");
  t("0",          "0",           "-0");
  t("0",          "0",           "-0", WRN_EXTRA_ZERO);
  t("+",          "0",          "-0+");
  t("-",          "0",         "-0+-");
  t("1",         "-1",        "-0+-1");
  t("-",         "-1",       "-0+-1-");
  t("+",         "-1",      "-0+-1-+");
  t("2",         "-3",     "-0+-1-+2");
  t("-",         "-3",    "-0+-1-+2-");
  t("-",         "-3",   "-0+-1-+2--");
  t("3",          "0",  "-0+-1-+2--3");

// repeated signs  (ignore third sign)
  t("C",           "",             "");
  t("-",           "0",           "-");
  t("-",           "0",          "--");
  t("-",           "0",          "--", ERR_EXP_USNUM);
  t("C",           "",             "");
  t("-",           "0",           "-");
  t("-",           "0",          "--");
  t("+",           "0",          "--", ERR_EXP_USNUM);
  t("C",           "",             "");
  t("-",           "0",           "-");
  t("+",           "0",          "-+");
  t("-",           "0",          "-+", ERR_EXP_USNUM);
  t("C",           "",             "");
  t("-",           "0",           "-");
  t("+",           "0",          "-+");
  t("+",           "0",          "-+", ERR_EXP_USNUM);
  t("C",           "",             "");
  t("+",           "0",           "+");
  t("-",           "0",          "+-");
  t("-",           "0",          "+-", ERR_EXP_USNUM);
  t("C",           "",             "");
  t("+",           "0",           "+");
  t("-",           "0",          "+-");
  t("+",           "0",          "+-", ERR_EXP_USNUM);
  t("C",           "",             "");
  t("+",           "0",           "+");
  t("+",           "0",          "++");
  t("-",           "0",          "++", ERR_EXP_USNUM);
  t("C",           "",             "");
  t("+",           "0",           "+");
  t("+",           "0",          "++");
  t("+",           "0",          "++", ERR_EXP_USNUM);

// only number can follow two signs
  t("C",           "",             "");
  t("+",           "0",           "+");
  t("-",           "0",          "+-");
  t("/",           "0",          "+-", ERR_EXP_USNUM);
  t("*",           "0",          "+-", ERR_EXP_USNUM);
  t("%",           "0",          "+-", ERR_EXP_USNUM);
  t("=",           "0",          "+-", ERR_EXP_USNUM);
  t("1",         "-1",          "+-1");

// decimals
  t("C",           "",             "");
  t("1",          "1",            "1");
  t(".",        "1.0",           "1.");
  t(".",        "1.0",           "1.", ERR_EXTRA_DECIMAL);
  t("0",        "1.0",          "1.0");
  t("0",        "1.0",         "1.00");
  t(".",        "1.0",         "1.00", ERR_EXTRA_DECIMAL);
  t("1",      "1.001",        "1.001");

// leading decimals
  t("C",           "",             "");
  t(".",        "0.0",            ".");
  t("1",        "0.1",           ".1");
  t("+",        "0.1",          ".1+");
  t(".",        "0.1",         ".1+.");
  t("2",        "0.3",        ".1+.2");

// trailing decimals not allowed
  t("C",           "",             "");
  t("1",          "1",            "1");
  t(".",         "1.0",          "1.");
  t("+",           "",           "1.", );


// adjacent operators
  t("C",           "",             "");
  t("*",           "",             "", ERR_EXP_OSNUM);
  t("/",           "",             "", ERR_EXP_OSNUM);
  t("%",           "",             "", ERR_EXP_OSNUM);
  t("=",           "",             "", ERR_EXP_OSNUM);
  t("0",          "0",            "0");
  t("+",          "0",           "0+");
  t("*",          "0",           "0+", ERR_EXP_OSNUM);
  t("/",          "0",           "0+", ERR_EXP_OSNUM);
  t("%",          "0",           "0+", ERR_EXP_OSNUM);
  t("=",          "0",           "0+", ERR_EXP_OSNUM);
  t("3",          "3",          "0+3");
  t("*",          "3",         "0+3*");
  t("+",          "3",        "0+3*+");
  t("+",          "3",        "0+3*+", ERR_EXP_USNUM);
  t("-",          "3",        "0+3*+", ERR_EXP_USNUM);
  t("*",          "3",        "0+3*+", ERR_EXP_USNUM);
  t("/",          "3",        "0+3*+", ERR_EXP_USNUM);
  t("%",          "3",        "0+3*+", ERR_EXP_USNUM);
  t("=",          "3",        "0+3*+", ERR_EXP_USNUM);

