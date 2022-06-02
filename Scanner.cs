using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lab1
{
    public enum SymbolType
    {
        DataType,
        Name,
        Equally,
        Argument,
        Сomma,
        Semicolon,
        Eof,
        Error
    }

    public struct Symbol
    {
        public SymbolType Type;
        public string Value;
        public int Index;
        public Symbol(SymbolType type = SymbolType.Eof, string value = " ", int index = 0)
        {
            Type = type;
            Value = value;
            Index = index;
        }
    }

    public class Scanner
    {
        private int _index = 0;
        private string _inputString;
        public int StringNum = 1;
        private int switcIndex = 0;
        private string result;
        private int temp_index_start;
        private int temp_index_last;
        public int mistakes_count = 0;
        private bool last_symbol = false;
        private bool not_false_symbol = true;
        public bool name_have = false;
        public string correct_str = "DataType NameEquallyArgumentSemicolon";
        public Scanner(string inputString)
        {
            _inputString = inputString;
            _index = 0;
        }

        public void Reset(int temp)
        {
            _index = temp;
        }
        public void Set_start_point(int temp_index)
        {
            if (temp_index > 0)
                temp_index_start = temp_index - 1;
            else
                temp_index_start = 0;
        }
        public void Set_last_point(int number = 0)
        {
            temp_index_last = _index - number;
        }

        public Symbol GetSymbol()
        {
            var value = String.Empty;
            int temp_index;
            while (_index < _inputString.Length && _inputString[_index] == ' ')
            {
                _index++;
            }
            temp_index = _index;
            while (!Eof)
            {
                value += _inputString[_index];
                _index++;

                if (Eof || value == ";")
                {
                    break;
                }
                if (_inputString[_index] == '\n')
                {
                    StringNum++;
                }
                if (_inputString[_index] == ' ')
                {
                    Set_start_point(temp_index);
                    break;
                }
            }
            return CheckSymbolType(value);
        }

        public Symbol CheckSymbolType(string value)
        {
            if (GetCommaSymbol(value).Type != SymbolType.Error)
            {
                return GetCommaSymbol(value);
            }

            if (GetDataTypeSymbol(value).Type != SymbolType.Error)
            {
                return GetDataTypeSymbol(value);
            }

            if (GetSemicolonSymbol(value).Type != SymbolType.Error)
            {
                return GetSemicolonSymbol(value);
            }

            if (GetArgumentSymbol(value).Type != SymbolType.Error)
            {
                return GetArgumentSymbol(value);
            }

            if (GetNameSymbol(value).Type != SymbolType.Error && switcIndex >= 1)
            {
                return GetNameSymbol(value);
            }

            if (GetEquallySymbol(value).Type != SymbolType.Error)
            {
                return GetEquallySymbol(value);
            }
            return new Symbol(SymbolType.Error, value);
        }
        public string Recursive_Syntaxis(bool exit = true)
        {
            string[] st = _inputString.Split(' ');
            int count = 0;
            foreach (string _s in st)
            {
                count++;
            }
            count--;
            Symbol symbol = GetSymbol();
            SymbolType prev_type = 0;
            SymbolType type;
            string message = "";

            switch (switcIndex)
            {
                case 0:
                    type = SymbolType.DataType;
                    message = "Ошибка 1. Ожидался тип данных, Получено: {0} Позиция:{1}";
                    if (not_false_symbol)
                        Set_last_point();
                    break;

                case 1:
                    type = SymbolType.Name;
                    message = "Ошибка 2. Ожидалось имя переменной, Получено: {0} Позиция:{1}";
                    if (not_false_symbol)
                        Set_last_point();
                    break;

                case 2:
                    type = SymbolType.Equally;
                    message = "Ошибка 3. Ожидался знак =, Получено: {0} Позиция:{1}";
                    if (not_false_symbol)
                        Set_last_point();
                    break;

                case 3:
                    type = SymbolType.Argument;
                    message = "Ошибка 4. Неверно задано значение, Получено: {0} Позиция:{1}";
                    if (not_false_symbol)
                        Set_last_point();
                    break;

                case 4:
                    type = SymbolType.Semicolon;
                    message = String.Format(" Ошибка 7. Ожидалась точка с запятой, Позиция:{0}", temp_index_last + 1);
                    if (not_false_symbol)
                        Set_last_point();
                    break;

                default:
                    type = SymbolType.Eof;
                    if (mistakes_count == 0)
                        result += String.Format("Ошибки отсутствуют") + "|";
                    else
                    {
                        correct_str = correct_str.Replace("DataType", "float");
                        correct_str = correct_str.Replace("Name", "name");
                        correct_str = correct_str.Replace("Equally", "=");
                        correct_str = correct_str.Replace("Argument", "113,1");
                        correct_str = correct_str.Replace("Semicolon", ";");
                        result += String.Format("Корректный вариант строки: ") + correct_str + "|";
                        mistakes_count = 0;
                    }
                        break;
            }
            if (type == SymbolType.Eof)
                return "";


            if (symbol.Type == type)
            {
                switcIndex++;
                not_false_symbol = true;
                correct_str = correct_str.Replace(Convert.ToString(symbol.Type), Convert.ToString(symbol.Value));
                Set_last_point();
                Recursive_Syntaxis();
            }
            else
            {
                switcIndex++;
                if (exit)
                {
                    result += String.Format(message, symbol.Value, temp_index_last) + "|";
                    last_symbol = false;
                    not_false_symbol = false;
                    mistakes_count++;
                    Set_last_point();
                    return Recursive_Syntaxis(false);
                }

                if (!exit)
                {
                    if (!last_symbol)
                        switcIndex--;
                    else switcIndex = switcIndex - 2;
                }
                not_false_symbol = false;
                if (type == SymbolType.Eof)
                {
                    return result;
                }
                Reset(temp_index_last);
                Recursive_Syntaxis();
            }
            return result;
        }

        private bool Eof
        {
            get
            {
                return _index == _inputString.Length;
            }
        }

        private Symbol GetEquallySymbol(string value)
        {
            if (value == "=")
            {
                return new Symbol(SymbolType.Equally, value, _index);
            }
            else
            {
                return GetErrorSymbol(_index, value);
            }
        }

        private Symbol GetNameSymbol(string value)
        {
            int i = 0;
            bool wrong_symbol = false;
            while (i < value.Length)
            {
                if (!IsLetter(value[i]) && !IsDigit(value[i]) || IsDigit(value[0]))
                {
                    wrong_symbol = true;
                }
                i++;
            }
            if ((value != String.Empty && !wrong_symbol) && (value != "double" || value != "float"))
            {
                return new Symbol(SymbolType.Name, value, _index);
            }
            else
            {
                return GetErrorSymbol(_index, value);
            }

        }


        private Symbol GetSemicolonSymbol(string value)
        {
            if (value == ";")
            {
                return new Symbol(SymbolType.Semicolon, value, _index);
            }
            else
            {
                return GetErrorSymbol(_index, value);
            }
        }
        private Symbol GetDataTypeSymbol(string value)
        {
            if (value == "double" || value == "float")
            {
                return new Symbol(SymbolType.DataType, value, _index);
            }
            else
            {
                return GetErrorSymbol(_index, value);
            }
        }


        private Symbol GetCommaSymbol(string value)
        {
            if (value == ",")
            {
                return new Symbol(SymbolType.Сomma, value, _index);
            }
            else
            {
                return GetErrorSymbol(_index, value);
            }
        }

        private Symbol GetErrorSymbol(int index, string value)
        {
            return new Symbol(SymbolType.Error, value, _index);
        }

        private Symbol GetArgumentSymbol(string value)
        {
            int i = 0;
            int comma = 0;
            bool not_digit = false;
            while (i < value.Length)
            {
                if (IsDigit(value[i]))
                {
                    i++;
                }
                else if (value[i] == ',')
                {
                    comma++;
                    i++;
                }
                else
                {
                    not_digit = true;
                    break;
                }
            }

            if (not_digit || value == String.Empty || comma != 1)
            {
                return GetErrorSymbol(_index, value);
            }
            else
            {
                return new Symbol(SymbolType.Argument, value, _index);
            }
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= 'а' && c <= 'я') || (c >= 'А' && c <= 'Я');
        }
    }
}