## COM741 Web Applications Development

### Practical 9

*At the end of this practical we will have implemented  user Authentication/Authorization to our application and added some additional security features (AntiForgery, Password Hashing and model binding)*

### Register User 
1. Complete the outline ```Register``` method in the ```StudentServiceDb``` class. It should search the database for a duplicate email address (emails should be unique) and if found return null, otherwise store new user and return as result of method. 

    ```
    public User Register(string name, string email, string password, Role role) { ... }
    ```

2. Complete the Register action in the UserController
a. Add ValidateAntiForgeryToken
b. Use the ```[Bind ..]``` attribute to bind only the Name, Email, Password and Role attributes from the UserViewModel (PasswordConfirm is not required)
c. Once registered redirect the user to the login page

3. Complete the ```UserViewModel``` by adding following validation attributes (listed in brackets)
    - Name (Required)  
    - Email (EmailAddress, Required)
    - Password (Required)
    - PasswordConfirm (Compare) -use the Compare validator to compare Password and PasswordConfirm fields.
    - Role (Required) - use a select list to populate the role from the values in the Role enumeration


4. Complete the ```Register.cshtml``` view in the Views/User folder to display a form that gathers values to populate the UserViewModel. For the role attribute, use a select list to populate the role from the values in the Role enumeration. For this we can use the ```Html.GetEnumSelectList``` method to populate the select component asp-items property as follows

    ```
    <select asp-for="Role" asp-items=Html.GetEnumSelectList<Role>()>
        ...
    </select>
    ```

    Build the application and attempt to Register a new user to verify the operations above, then modify Seeder to create three demo users  - one for each role: "admin@sms.com", "manager@sms.com", and "guest@sms.com” (use passwords admin, manager, guest)

    ### Authorization
5. Add required Authorization attributes to enforce the following authorisation rules:

    - StudentController actions
        - User must be authenticated to access Student actions
        - Only users with admin role can create a Student
        - Only users with admin role can delete a Student
        - Only users with admin or manager role can edit a Student
        - An authenticated user can create or delete a ticket

    - TicketController actions
        - User must be authenticated to access Ticket actions
        - Only users with admin or manager role can create or close a ticket 

6. Modify the Student views to hide buttons/links not accessible to users based on authorisations defined above. For example to only display the create student button on the index page if the user logged in has either 'manager' or 'admin' role

    ```
    @if (User.IsInRole("admin") || User.IsInRole("manager"))
    {
        <a class="btn btn-sm btn-primary my-3" 
           asp-controller="Student" asp-action="Create">Create</a>     
    }
    ```

    Consider using the ```asp-condition``` tagHelper found in the TagHelpers folder, rather than @if shown above. Further, @if else statements such as following code which displays an icon based on a tickets active status

    ```
    @if (@t.Active)
    {
        <i class="xxxxx" ></i>
    }
    else
    {
        <i class="xxxxx"></i>
    }
    ```
    could be replaced with

    ```
    <i class="xxxx" asp-condition="@t.Active"></i>
    <i class="xxxx" asp-condition="@(!t.Active)"></i>
    ```

    ### OPTIONAL (we will cover this next week)

7. Review the documentation below for use of the Remote validator. This provides client side validation to call a remote action on a controller. We could use this to call the service to verify if the email address the user just entered is unique (not already used), as opposed to only checking the email address when the form is submitted to the server. This would provide more immediate feedback to the user when registering.
You would need to do the following as outlined in the Remote validator documentation:
    - Add the ```[Remote]``` attribute to the ```Email``` property in the UserRegisterViewModel.
    - Add a new action to the ```UserController``` which can be called to validate the email.
    - Remove the manual check for a unique email in the Register action

```https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-5.0#remote-attribute```. 

*Note the Remote validator should only be used in ViewModels as its part of the .NET MVC library and we don't want this to be a dependency in our Data project.*