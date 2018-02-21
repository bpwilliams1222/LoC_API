using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoCWebApp.Models
{

    public class MessageFormattingModels
    {
        public static int DetermineSpacing(int maxLineLength, int contentLen)
        {
            int spaces = 2;

            if ((maxLineLength - contentLen) > 0)
            {
                spaces = maxLineLength - contentLen;
            }

            return spaces;
        }

        public static string AddSpacesToMessage(string message, int spaces)
        {
            string messageWithSpaces = "";

            if (spaces % 2 == 0)
            {
                // if even numbers half before message and half after
                for (int i = 0; i < spaces / 2; i++)
                {
                    messageWithSpaces += " ";
                }
                messageWithSpaces += message;
                for (int i = 0; i < spaces / 2; i++)
                {
                    messageWithSpaces += " ";
                }
            }
            else
            {
                for (int i = 0; i < spaces / 2; i++)
                {
                    messageWithSpaces += " ";
                }
                messageWithSpaces += message;
                for (int i = 0; i < (spaces / 2) + 1; i++)
                {
                    messageWithSpaces += " ";
                }
            }

            return messageWithSpaces;
        }

        public static string ConvertNumberToAbrNumberString(long number)
        {
            string convertedNumberString = "";

            if (number < 1000)
            {
                convertedNumberString = number.ToString();
            }
            else if (number <= 1000000)
            {
                convertedNumberString = (number / 1000.00).ToString("N1") + "K";
            }
            else if (number <= 1000000000)
            {
                convertedNumberString = (number / 1000000.00).ToString("N1") + "M";
            }
            else
            {
                convertedNumberString = (number / 1000000000.00).ToString("N1") + "B";
            }

            return convertedNumberString;
        }

        public static string ConvertLoginRangeToString(loginRanges range)
        {
            switch (range)
            {
                case loginRanges.hour00to01:
                    return "00:00-01:00";
                case loginRanges.hour01to02:
                    return "01:00-02:00";
                case loginRanges.hour02to03:
                    return "02:00-03:00";
                case loginRanges.hour03to04:
                    return "03:00-04:00";
                case loginRanges.hour04to05:
                    return "04:00-05:00";
                case loginRanges.hour05to06:
                    return "05:00-06:00";
                case loginRanges.hour06to07:
                    return "06:00-07:00";
                case loginRanges.hour07to08:
                    return "07:00-08:00";
                case loginRanges.hour08to09:
                    return "08:00-09:00";
                case loginRanges.hour09to10:
                    return "09:00-10:00";
                case loginRanges.hour10to11:
                    return "10:00-11:00";
                case loginRanges.hour11to12:
                    return "11:00-12:00";
                case loginRanges.hour12to13:
                    return "12:00-13:00";
                case loginRanges.hour13to14:
                    return "13:00-14:00";
                case loginRanges.hour14to15:
                    return "14:00-15:00";
                case loginRanges.hour15to16:
                    return "15:00-16:00";
                case loginRanges.hour16to17:
                    return "16:00-17:00";
                case loginRanges.hour17to18:
                    return "17:00-18:00";
                case loginRanges.hour18to19:
                    return "18:00-19:00";
                case loginRanges.hour19to20:
                    return "19:00-20:00";
                case loginRanges.hour20to21:
                    return "20:00-21:00";
                case loginRanges.hour21to22:
                    return "21:00-22:00";
                case loginRanges.hour22to23:
                    return "22:00-23:00";
                case loginRanges.hour23to24:
                    return "23:00-00:00";
                default:
                    return "00:00-01:00";
            }
        }
    }
}