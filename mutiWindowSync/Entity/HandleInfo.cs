using System;

namespace mutiWindowSync.Entity
{
    public class HandleInfo : BaseDto
    {
        /**
         * 句柄ID
         */
        private string handleId;

        public string HandleId
        {
          get => handleId;
          set
          {
           value ??= String.Empty;
           SetProperty(ref handleId, value);
           RaisePropertyChanged();
          }
        }
        
        public IntPtr Handle { get; set; }
        /**
         * 线程状态
         */
        private string state;
        
        public string State
        {
          get => state;
          set
          {
           value ??= String.Empty;
           SetProperty(ref state, value);
           RaisePropertyChanged();
          }
        }
        /**
         * 句柄标题
         */
        private string title;
        
        public string Title
        {
          get => title;
          set
          {
           value ??= String.Empty;
           SetProperty(ref title, value);
           RaisePropertyChanged();
          }
        }
        /**
         * 窗口类型
         */
        private string type = "同步端";
        
        public string Type
        {
          get => type;
          set
          {
           value ??= String.Empty;
           SetProperty(ref type, value);
           RaisePropertyChanged();
          }
        }
        /**
         * 窗口大小
         */
        private string size;
        
        public string Size
        {
          get => size;
          set
          {
           value ??= String.Empty;
           SetProperty(ref size, value);
           RaisePropertyChanged();
          }
        }
        
    }
}