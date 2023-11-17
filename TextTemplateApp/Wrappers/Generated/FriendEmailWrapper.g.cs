namespace TextTemplateApp
{
    using ModelWrapperBase;
    using System;
    using System.Linq;
    using TestModels;
     
    public partial class FriendEmailWrapper : ModelWrapper<FriendEmail>
    {
        public FriendEmailWrapper(FriendEmail model) : base(model)
        {
        }
         
        public System.String Comment
        {
            get => GetValue<System.String>();
            set => SetValue(value);
        }
         
        public bool CommentIsChanged => GetIsChanged(nameof(Comment));
         
        public System.String CommentOriginalValue => GetOriginalValue<System.String>(nameof(Comment));
         
        public System.String Email
        {
            get => GetValue<System.String>();
            set => SetValue(value);
        }
         
        public bool EmailIsChanged => GetIsChanged(nameof(Email));
         
        public System.String EmailOriginalValue => GetOriginalValue<System.String>(nameof(Email));
         
        public System.Int32 Id
        {
            get => GetValue<System.Int32>();
            set => SetValue(value);
        }
         
        public bool IdIsChanged => GetIsChanged(nameof(Id));
         
        public System.Int32 IdOriginalValue => GetOriginalValue<System.Int32>(nameof(Id));
    }
}
