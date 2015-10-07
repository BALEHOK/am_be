using AppFramework.Entities;

namespace AppFramework.Core.Classes.ScreensServices
{
    public class Layout
    {
        private readonly LayoutType _type;
        private readonly string _name;
        private readonly int _id;
        public int Id
        {
            get { return _id; }
        }
        public string Name
        {
            get { return _name; }
        }

        public LayoutType Type
        {
            get { return _type; }
        }

        public Layout(ScreenLayout data)
        {
            _id = data.Id;
            _type = (LayoutType)data.Type;
            _name = data.Name;
        }
    }
}
