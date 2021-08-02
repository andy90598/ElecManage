using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElecManage.TimerFeatures
{
    public class TimerManager
    {
        private Timer _timer;
        private AutoResetEvent _autoResetEvent;
        //用Action委託每兩秒執行一次傳遞的回調函數
        private Action _action;
        public DateTime TimeStarted { get; }

        public TimerManager(Action action)
        {
            _action = action;
            _autoResetEvent = new AutoResetEvent(false);
            //計時器將在第一次執行前暫停一秒鐘 ， 每兩秒執行一次
            _timer = new Timer(Execute, _autoResetEvent, 1000, 2000);
            TimeStarted = DateTime.Now;
        }
        public void Execute(object stateInfo)
        {
            _action();
            //執行60秒
            if((DateTime.Now - TimeStarted).Seconds>60)
            {
                _timer.Dispose();
            }
        }
    }
}
