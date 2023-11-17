namespace TextTemplateApp
{
    using ModelWrapperBase;
    using TestModels;
     
    public partial class FriendWrapper : ModelWrapper<Friend>
    {
        public FriendWrapper(Friend model) : base(model)
        {
        }
         
        public System.Nullable<System.DateTime> Birthday
        {
            get => GetValue<System.Nullable<System.DateTime>>();
            set => SetValue(value);
        }
         
        public bool BirthdayIsChanged => GetIsChanged(nameof(Birthday));
         
        public System.Nullable<System.DateTime> BirthdayOriginalValue => GetOriginalValue<System.Nullable<System.DateTime>>(nameof(Birthday));
         
        public System.String FirstName
        {
            get => GetValue<System.String>();
            set => SetValue(value);
        }
         
        public bool FirstNameIsChanged => GetIsChanged(nameof(FirstName));
         
        public System.String FirstNameOriginalValue => GetOriginalValue<System.String>(nameof(FirstName));
         
        public System.Int32 FriendGroupId
        {
            get => GetValue<System.Int32>();
            set => SetValue(value);
        }
         
        public bool FriendGroupIdIsChanged => GetIsChanged(nameof(FriendGroupId));
         
        public System.Int32 FriendGroupIdOriginalValue => GetOriginalValue<System.Int32>(nameof(FriendGroupId));
         
        public System.Int32 Id
        {
            get => GetValue<System.Int32>();
            set => SetValue(value);
        }
         
        public bool IdIsChanged => GetIsChanged(nameof(Id));
         
        public System.Int32 IdOriginalValue => GetOriginalValue<System.Int32>(nameof(Id));
         
        public System.Boolean IsDeveloper
        {
            get => GetValue<System.Boolean>();
            set => SetValue(value);
        }
         
        public bool IsDeveloperIsChanged => GetIsChanged(nameof(IsDeveloper));
         
        public System.Boolean IsDeveloperOriginalValue => GetOriginalValue<System.Boolean>(nameof(IsDeveloper));
         
        public System.String LastName
        {
            get => GetValue<System.String>();
            set => SetValue(value);
        }
         
        public bool LastNameIsChanged => GetIsChanged(nameof(LastName));
         
        public System.String LastNameOriginalValue => GetOriginalValue<System.String>(nameof(LastName));
         
        public AddressWrapper Address { get; private set; }
         
        public ChangeTrackingCollection<FriendEmailWrapper> Emails { get; private set; }
         
        protected override void InitializeComplexProperties()
        {
            if (Model.Address is null)
            {
                throw new ArgumentException("Address must not be null.");
            }
             
            Address = new AddressWrapper(Model.Address);
            RegisterComplex(Address);
        }
         
        protected override void InitializeCollectionProperties()
        {
            if (Model.Emails is null)
            {
                throw new ArgumentException("Emails must not be null.");
            }
             
            Emails = new ChangeTrackingCollection<FriendEmailWrapper>(Model.Emails.Select(e => new FriendEmailWrapper(e)));
            RegisterCollection(Emails, Model.Emails);
        }
    }
}
