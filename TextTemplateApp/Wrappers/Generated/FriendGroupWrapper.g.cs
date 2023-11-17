namespace TextTemplateApp
{
    using ModelWrapperBase;
    using TestModels;
     
    public partial class FriendGroupWrapper : ModelWrapper<FriendGroup>
    {
        public FriendGroupWrapper(FriendGroup model) : base(model)
        {
        }
         
        public System.Int32 Id
        {
            get => GetValue<System.Int32>();
            set => SetValue(value);
        }
         
        public bool IdIsChanged => GetIsChanged(nameof(Id));
         
        public System.Int32 IdOriginalValue => GetOriginalValue<System.Int32>(nameof(Id));
         
        public System.String Name
        {
            get => GetValue<System.String>();
            set => SetValue(value);
        }
         
        public bool NameIsChanged => GetIsChanged(nameof(Name));
         
        public System.String NameOriginalValue => GetOriginalValue<System.String>(nameof(Name));
    }
}
