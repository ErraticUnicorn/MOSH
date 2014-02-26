using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SunsetHigh
{
    public sealed class NotificationPanel : TextPanel
    {
        private const int DEFAULT_VIEWPORT_WIDTH = 800;
        private const int DEFAULT_PANEL_HEIGHT = 50;
        private const int DEFAULT_PANEL_WIDTH = 150;
        private const int DEFAULT_X_MARGIN = 15;
        private const int DEFAULT_Y_MARGIN = 15;
        private const float SHOW_TIME = 1.0f;   //in seconds
        private const float POP_TIME = 0.5f;

        private float showNotificationTimer = 0.0f;
        private volatile Queue<string> notificationQueue;

        //Singleton implementation
        private static volatile NotificationPanel inst;
        private static object syncRoot = new Object();
        /// <summary>
        /// Returns an instance of LocationNamePanel
        /// </summary>
        public static NotificationPanel instance
        {
            get
            {
                if (inst == null)
                {
                    lock (syncRoot)
                    {
                        if (inst == null)
                            inst = new NotificationPanel();
                    }
                }
                return inst;
            }
        }

        private NotificationPanel()
            : base(DEFAULT_VIEWPORT_WIDTH - DEFAULT_PANEL_WIDTH, -DEFAULT_PANEL_HEIGHT, 
            DEFAULT_PANEL_WIDTH, DEFAULT_PANEL_HEIGHT)
        {
            this.setPopDuration(POP_TIME);
            this.setPopLocations(DEFAULT_VIEWPORT_WIDTH - DEFAULT_PANEL_WIDTH, -DEFAULT_PANEL_HEIGHT,
                DEFAULT_VIEWPORT_WIDTH - DEFAULT_PANEL_WIDTH, 0);
            this.setXMargin(DEFAULT_X_MARGIN);
            this.setYMargin(DEFAULT_Y_MARGIN);
            this.notificationQueue = new Queue<string>();
            //register event listeners
            Quest.QuestStateChanged += questEventListener;
            Hero.instance.inventory.InventoryChanged += inventoryEventListener;
        }

        public override void reset()
        {
            base.reset();
            this.setX(this.getHideX());
            this.setY(this.getHideY());
            showNotificationTimer = 0.0f;
            this.popOut();
            this.setVisible(false);
        }

        public override void setMessage(string message)
        {
            base.setMessage(message);
            if (font != null)
            {
                int width = (int)font.MeasureString(message).X + this.getXMargin() * 2;
                int oldWidth = this.getWidth();
                int displacement = oldWidth - width;
                this.setWidth(width);
                this.setX(this.getX() + displacement);
                this.setHideX(this.getHideX() + displacement);
                this.setAppearX(this.getAppearX() + displacement);
            }
        }

        public override void update(float elapsed)
        {
            base.update(elapsed);

            if (notificationQueue.Count > 0 && this.getX() == this.getHideX() && this.getY() == this.getHideY())
            {
                this.setMessage(notificationQueue.Dequeue());
                popIn();
            }
            if (this.getX() == this.getAppearX() && this.getY() == this.getAppearY())
                showNotificationTimer += elapsed;
            if (showNotificationTimer > SHOW_TIME)
            {
                popOut();
                showNotificationTimer = 0.0f;
            }
        }

        public void pushNotification(string notification)
        {
            notificationQueue.Enqueue(notification);
        }

        private void questEventListener(object sender, QuestEventArgs e)
        {
            this.pushNotification("Updated Quest " + SunsetUtils.enumToString<QuestID>(e.questID));
        }

        private void inventoryEventListener(object sender, InventoryEventArgs e)
        {
            if (e.quantity > 0)
            {
                this.pushNotification("Received " + SunsetUtils.enumToString<Item>(e.type) + "(" + e.quantity + ")");
            }
        }
    }
}
