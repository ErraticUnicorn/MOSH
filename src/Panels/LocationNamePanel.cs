using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SunsetHigh
{
    public sealed class LocationNamePanel : TextPanel
    {
        private const int DEFAULT_PANEL_HEIGHT = 50;
        private const int DEFAULT_PANEL_WIDTH = 150;
        private const int DEFAULT_X_MARGIN = 15;
        private const int DEFAULT_Y_MARGIN = 15;
        private const float SHOW_TIME = 1.0f;   //in seconds
        private const float POP_TIME = 0.5f;

        private float showLocationTimer = 0.0f;

        //Singleton implementation
        private static volatile LocationNamePanel inst;
        private static object syncRoot = new Object();
        /// <summary>
        /// Returns an instance of LocationNamePanel
        /// </summary>
        public static LocationNamePanel instance
        {
            get
            {
                if (inst == null)
                {
                    lock (syncRoot)
                    {
                        if (inst == null)
                            inst = new LocationNamePanel();
                    }
                }
                return inst;
            }
        }

        private LocationNamePanel()
            : base(0, -DEFAULT_PANEL_HEIGHT, DEFAULT_PANEL_WIDTH, DEFAULT_PANEL_HEIGHT)
        {
            this.setPopDuration(POP_TIME);
            this.setPopLocations(0, -DEFAULT_PANEL_HEIGHT, 0, 0);
            this.setXMargin(DEFAULT_X_MARGIN);
            this.setYMargin(DEFAULT_Y_MARGIN);
        }

        public override void reset()
        {
            base.reset();
            this.setX(this.getHideX());
            this.setY(this.getHideY());
            showLocationTimer = 0.0f;
            this.popOut();
        }

        public override void setMessage(string message)
        {
            base.setMessage(message);
            if (font != null)
            {
                int width = (int)font.MeasureString(message).X;
                if (width > DEFAULT_PANEL_WIDTH)
                    this.setWidth(this.getXMargin() * 2 + width);
                else
                    this.setWidth(DEFAULT_PANEL_WIDTH);
            }
        }

        public override void update(float elapsed)
        {
            base.update(elapsed);

            if (this.getX() == this.getAppearX() && this.getY() == this.getAppearY())
                showLocationTimer += elapsed;
            if (showLocationTimer > SHOW_TIME)
            {
                popOut();
                showLocationTimer = 0.0f;
            }
        }

        public void showNewLocation(string locationName)
        {
            reset();
            setMessage(locationName);
            popIn();
        }
    }
}
