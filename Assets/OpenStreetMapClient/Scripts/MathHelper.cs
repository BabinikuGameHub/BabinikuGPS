using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSMClient
{
    public static class MathHelper
    {
        public static IEnumerable<(int x, int y)> GetSpiralFromCenter(int width, int height)
        {
            return GetSpiralFromCenter(0, 0, width, height);
        }

        public static IEnumerable<(int x, int y)> GetSpiralFromCenter(int startX, int startY, int width, int height)
        {
            var radius = Math.Max(width, height) / 2;
            var centerX = startX + width / 2;
            var centerY = startY + height / 2;

            var boundX = startX + width;
            var boundY = startY + height;

            if (Check(centerX, centerY, out var p)) yield return p;

            for (var r = 1; r <= radius; r++)
            {
                var fromX = centerX - r;
                var fromY = centerY - r;
                var toX = centerX + r;
                var toY = centerY + r;

                for (int x = 0; x <= r; x++)
                {
                    if (Check(centerX + x, fromY, out p)) yield return p;
                    if (Check(centerX + x, toY, out p)) yield return p;
                    if (x > 0)
                    {
                        if (Check(centerX - x, fromY, out p)) yield return p;
                        if (Check(centerX - x, toY, out p)) yield return p;
                    }
                }

                for (int y = 0; y < r; y++)
                {
                    if (Check(fromX, centerY + y, out p)) yield return p;
                    if (Check(toX, centerY + y, out p)) yield return p;
                    if (y > 0)
                    {
                        if (Check(fromX, centerY - y, out p)) yield return p;
                        if (Check(toX, centerY - y, out p)) yield return p;
                    }
                }
            }

            bool Check(int xx, int yy, out (int, int) point)
            {
                point = (xx, yy);
                return xx >= startX && xx < boundX && yy >= startY && yy < boundY;
            }
        }
    }
}