**NAVNEGUIDEN.DK REST API DOKUMENTATION**
----

**Show User**

Get User object of logged in user (for profile page)

* **URL**

/user

* **Method:**
  
  <_The request type_>

`GET`
  
*  **URL Params**



   **Required:**
 
`iUserRepository=[IUserRepository]`


* **Data Params**



* **Success Response:**
  


  * **Code:** 200 <br />
    **Content:** `{id, email, firstname, isadmin}`
 
* **Error Response:**

  

  * **Code:** 401 UNAUTHORIZED <br />
    **Content:** `{ error :  }`

  OR

  * **Code:** 422 UNPROCESSABLE ENTRY <br />
    **Content:** `{ error : "Email Invalid" }`


* **Notes:**

  
