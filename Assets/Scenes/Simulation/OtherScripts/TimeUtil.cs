using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeUtil {
    
    public static string ConvertHoursToString(long hours) {
        if (hours >= 8640) {
            long years = hours / 8640;
            hours = hours % 8640;
            long month = hours / 720;
            hours = hours % 720;
            long weeks = hours / 168;
            hours = hours % 168;
            long days = hours / 24;
            hours = hours % 24;
            return years + "y, " + month + "m, " + weeks + "w, " + days + "d, " + hours + "h";
        } else if (hours >= 720) {
            long month = hours / 720;
            hours = hours % 720;
            long weeks = hours / 168;
            hours = hours % 168;
            long days = hours / 24;
            hours = hours % 24;
            return month + "m, " + weeks + "w, " + days + "d, " + hours + "h";
        } else if (hours >= 168) {
            long weeks = hours / 168;
            hours = hours % 168;
            long days = hours / 24;
            hours = hours % 24;
            return weeks + "w, " + days + "d, " + hours + "h";
        } else if (hours >= 24) {
            long days = hours / 24;
            hours = hours % 24;
            return days + "d, " + hours + "h";
        } else {
            return hours + "h";
        }
    }

    public static string ConvertHoursToDecimalString(long hours) {
        if (hours < 24)
            return hours + "h";
        else if (hours < 168)
            return (long)(hours * 10.0f / 24) / 10.0f + "d";
        else if (hours < 720)
            return (long)(hours * 10.0f / 168) / 10.0f + "w";
        else if (hours  < 8640)
            return (long)(hours * 10.0f / 720) / 10.0f + "m";
        else
            return (long)(hours  * 10.0f / 8640) / 10.0f + "y";
    }
}
