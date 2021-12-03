using System;
using System.Linq;
using System.Collections.Generic;

class pling
{
    static Dictionary<string, dynamic> variables = new Dictionary<string, dynamic>();
    static Stack<string> activefunc = new Stack<string>();
    static Stack<string> activeclosure = null;
    static int closuredepth = 0;
    static string symbolstr  = "():.;*+-/?=<>!&|%";
    static Stack<dynamic> stack = new Stack<dynamic>();

    static void Main(string[] args)
    {
        Console.WriteLine("pling REPL v.1.0");
        bool insinglequotes = false;
        bool indoublequotes = false;
        while(true) {
            Console.Write("> ");
            process_line(Console.ReadLine(), ref insinglequotes, ref indoublequotes);
        }
    }
    
    static void process_line(string line, ref bool insinglequotes, ref bool indoublequotes) {
        try {
            string token = "";
            foreach(char c in line) {
                if(c == '"' && !insinglequotes) indoublequotes = !indoublequotes;
                if(c == '\'' && !indoublequotes) insinglequotes = !insinglequotes;
                if(indoublequotes || insinglequotes) token += c;
                else if(char.IsWhiteSpace(c)) {
                    process(token);
                    token = "";
                } else if(symbolstr.Contains(c)) {
                    process(token);
                    process("" + c);
                    token = "";
                } else token += c;
            }
            process(token);
        } catch(Exception e) {
            Console.WriteLine(e.Message);
        }
    }

    static dynamic eval(Stack<dynamic> stack) {
        if(stack.Peek() is Stack<string>) {
            Stack<string> top = stack.Pop();
            foreach(var s in top.Reverse()) process(s);
        }
        return stack.Count > 0 ? stack.Pop() : null;
    }

    static void swaptop(Stack<dynamic> stack) {
        var (t1, t2) = (stack.Pop(), stack.Pop());
        stack.Push(t1);
        stack.Push(t2);
    }

    static string disp(dynamic o) => o switch {
        bool b => b ? "#t" : "#f",
        Stack<string> s => $"({string.Join(" ", s.Reverse().ToArray())})",
        string s => $"'{s}'",
        _ => o.ToString()
    };

    static void process(string token) {
        if(token == "") return;
        if(activeclosure != null) {
            if(token == "(") closuredepth++;
            else if(token == ")" && closuredepth > 0) closuredepth--;
            else if(token == ")") {
                stack.Push(activeclosure);
                activeclosure = null;
                return;
            }
            activeclosure.Push(token);
        } else if(int.TryParse(token, out int r)) {
            stack.Push(r);
        } else if(double.TryParse(token, out double d)) {
            stack.Push(d);
        } 
        else if("+-*/%=<>&|".Contains(token)) {
            swaptop(stack);
            stack.Push(token switch {
                "+" => eval(stack) + eval(stack),
                "-" => eval(stack) - eval(stack),
                "*" => eval(stack) * eval(stack),
                "/" => eval(stack) / eval(stack),
                "%" => eval(stack) % eval(stack),
                "=" => eval(stack) == eval(stack),
                "<" => eval(stack) < eval(stack),
                ">" => eval(stack) > eval(stack),
                "&" => eval(stack) ? eval(stack) : !stack.TryPop(out dynamic _),
                "|" => eval(stack) ? stack.TryPop(out dynamic _) : eval(stack),
                _ => null
            });
        } else if(token == ":") {
            stack.Push(stack.Peek());
        } else if(token == ".") {
            stack.Pop();
        } else if(token == ";") {
            swaptop(stack);
        } else if(token == "!") {
            stack.Push(!eval(stack));  
        } else if(token == "?") {
            dynamic op2 = stack.Pop();
            dynamic op1 = stack.Pop();
            if(eval(stack) switch {
                bool b => b,
                null => false,
                0 => false,
                _ => true
            }) {
                stack.Push(op1);
                stack.Push(eval(stack));
            } else { 
                stack.Push(op2);
                stack.Push(eval(stack));
            }
        } else if(token == "(") {
            activeclosure = new Stack<string>();
        } else if(token == "#t") {
            stack.Push(true);
        } else if(token == "#f") {
            stack.Push(false);
        } else if(token == "disp") {
            Console.WriteLine(disp(stack.Peek()));
        } else if(token == "stack") {
            if(stack.Count == 0) Console.WriteLine("Stack empty.");
            else {
                Console.WriteLine(disp(stack.Peek()) + " <- top");
                foreach(var e in stack.Skip(1)) Console.WriteLine(disp(e));
            }
        } else if(token == "run") {
            if(stack.Peek() is string)  {
                bool q0 = false, q1 = false;
                process_line(stack.Pop(), ref q0, ref q1);
            }  
            else Console.WriteLine("Can only run strings.");
        } else if(token.Length > 2 && ((token[0] == '\'' && token[token.Length - 1] == '\'') || (token[0] == '"' && token[token.Length - 1] == '"'))) {
            stack.Push(token.Substring(1, token.Length - 2));
        } else {
            if(token[0] == '$' && token.Length > 1) {
                variables[token.Substring(1)] = stack.Pop();
            }
            else if(variables.ContainsKey(token)) {
                stack.Push(variables[token]);
                dynamic result = eval(stack);
                if(result != null) stack.Push(result);
            } else {
                Console.WriteLine("No variable found.");
            }
        }
    }
}