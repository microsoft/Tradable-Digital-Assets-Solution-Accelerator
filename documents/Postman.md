# API Testing Using Postman

This documentation contains information on how to test APIs for **Azure Tradable Digital Assets Solution Accelerators** using Postman. Additionally, you can refer to sequence diagrams stored in the **documents/media/Process_Sequences** directory of this solution to know the order of API calls work together. 

## How to use Postman
1. Download and install [Postman](https://www.postman.com/) if not already, and then run it.

1. Load Postman Collection [Contoso_DigitalGoods_Environment.postman_environment.json](/documents/Postman/Contoso_DigitalGoods_Environment.postman_environment.json).

1. Load Postman Environment Collection [Contoso_DigitalGoods.postman_collection.json](/documents/Postman/Contoso_DigitalGoods.postman_collection.json).

    ![alt text](  /documents/media/Postman/Postman_00.PNG)

1. Click on the **EYE** button and then **Edit** to Configure the Environment variables. 

    ![alt text](  /documents/media/Postman/Postman_01.PNG)

1. Configure the Postman Environment.
    - SERVICE_ENDPOINT_IP : Use the **Contoso Application App** service IP address received at the end of [Tradable Digital Asset](/deployment/Application_Deployment/README.md) deployment.
    - TOKEN_SERVICE_ENDPOINT_IP: Use the **Contoso Token Service API** service IP address received at the end of [Tradable Digital Asset](/deployment/Application_Deployment/README.md) deployment.
        - **Note:** Remove  **http://** from the application endpoint and token service endpoint addresses.

    ![alt text](  /documents/media/Postman/Postman_02.PNG)

1. Provision User by clicking on **DigitalGood App - UserProfile - Provisioning User by Contoso Identifier** and click **Send**.
    - **Copy the response. This information will be needed later.**

    ![alt text](  /documents/media/Postman/Postman_03.PNG)
1. Now Change following Environment variables and also Configure product variables.
    - PRODUCTID : Any unique values
    - TITLE : Product Title
    - SUB_TITLE: Product Description
    - DESCRIPTION: Product Detail Description
    - nftServiceUserID: nftServiceUserID received in Step 6.
    - ContosoIdentifier: contosoID received in Step 6

    ![alt text](  /documents/media/Postman/Postman_04.PNG)
1. Now Register product by clicking on **DigitalGood App - ProductCatalog - RegisterProductInfo**.
    - In the body section set the **url** value (within **assets**) to be your preferred URL of a product image (any live image URL copied from the internet should work)
    - Press send
    - A sucessful response will include product details

    ![alt text](  /documents/media/Postman/Postman_05.PNG)
1. Now convert product to a digital good by clicking on **DigitalGood TokenAPI - Product Factory - MakeDigitalGood** to generate a token number
    - In the body section set the **imageURL** value to the URL which used when you registered the product in Step 8
    - Press send
    - A sucessful response will include digital product and token number.
    - **Note:** Remove the leading zeros from the **tokenNumber** value and used the remaing value, e.g. '2' as the token  number in the next step

    ![alt text](  /documents/media/Postman/Postman_11.PNG)
1. Now Create gift by clicking on **DigitalGood App - Gift Management - Create Gift**.
    - Update the ContosUserID value using the ContosoID value recieved in Step 6
    - Change the TokenID value in the request which recieved in Step 9

    ![alt text](  /documents/media/Postman/Postman_06.PNG)
1. After a successful response Configure **GIFTID** in Manage Environments.

    ![alt text](  /documents/media/Postman/Postman_07.PNG)
1. Generate Gift URL by clicking on **DigitalGood App - Gift Management - Generate Gift link**.

    ![alt text](  /documents/media/Postman/Postman_08.PNG)
1. Paste the Gift URL into a browser.

    ![alt text](  /documents/media/Postman/Postman_09.PNG)
1. Once the URL has loaded in the browser, go to postman and click **DigitalGood App - DigitalLocker - Get DigitalLocker Item By TokenNumber** to get the digital locker data.

    ![alt text](  /documents/media/Postman/Postman_10.PNG)   

