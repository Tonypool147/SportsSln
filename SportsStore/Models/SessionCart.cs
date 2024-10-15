using System.Text.Json.Serialization;
using SportsStore.Infrastructure;

namespace SportsStore.Models {

    // this class has been implemented in order to create a Session object
    // within the Cart Model
    public class SessionCart : Cart 
    {
        public static Cart GetCart(IServiceProvider services) 
        {
            // get the ISession Object
            ISession? session = services.GetRequiredService<IHttpContextAccessor>().HttpContext?.Session;
            // get the cart session object of the current class type (SessionCart)
            // or create it if it does not exist
            SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? new SessionCart();
            // set this cart class session member
            cart.Session = session;
            return cart;
        }

        // The Session object used o store Cart data
        [JsonIgnore]
        public ISession? Session { get; set; }

        public override void AddItem(Product product, int quantity) {
            base.AddItem(product, quantity);

            // calls the SessionsExtensions method SetJson
            Session?.SetJson("Cart", this);
        }

        public override void RemoveLine(Product product) {
            base.RemoveLine(product);

            // calls the SessionsExtensions method SetJson
            Session?.SetJson("Cart", this);
        }

        public override void Clear() {
            base.Clear();

            // calls the SessionsExtensions method SetJson
            Session?.Remove("Cart");
        }
    }
}
