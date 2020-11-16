            using DoubleKPhoneShop.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DoubleKPhoneShop.Startup))]
namespace DoubleKPhoneShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
        }
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {
                // first we create Admin rool   
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website       
                var user = new ApplicationUser();
                user.UserName = "firstAdmin@gmail.com";
                user.Email = "firstAdmin@gmail.com";


                string userPWD = "Admin@123";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }
            }

            // creating Creating Guest role    
            if (!roleManager.RoleExists("Guest"))
            {
                var role = new IdentityRole();
                role.Name = "Guest";
                roleManager.Create(role);

            }

            //// creating Creating Employee role    
            //if (!roleManager.RoleExists("Employee"))
            //{
            //    var role = new IdentityRole();
            //    role.Name = "Employee";
            //    roleManager.Create(role);

            //    var user = new ApplicationUser();
            //    user.UserName = "Employee1@gmail.com";
            //    user.Email = "Employee1@gmail.com";
            //    string userPWD = "Employee1";
            //    var chkUser = UserManager.Create(user, userPWD);

            //    if (chkUser.Succeeded)
            //    {
            //        var result1 = UserManager.AddToRole(user.Id, "Employee");
            //    }
            //}
        }
    }
}
