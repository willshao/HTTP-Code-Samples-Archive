<?php

class AccessTokenAuthentication {
    /*
     * Get the access token.
     *
     * @param string $grantType    Grant type.
     * @param string $scopeUrl     Application Scopr URL.
     * @param string $clientID     Application client ID.
     * @param string $clientSecret Application client ID.
     * @param string $authUrl      Oauth Url.
     *
     * @return string.
     */
    function getTokens($grantType, $scopeUrl, $clientID, $clientSecret, $authUrl){
        try {
            //Initialize the Curl Session.
            $ch = curl_init();
            //Create the request Array.
            $paramArr = array (
                 'grant_type'    => $grantType,
                 'scope'         => $scopeUrl,
                 'client_id'     => $clientID,
                 'client_secret' => $clientSecret
            );
            //Create an Http Query.//
            $paramArr = http_build_query($paramArr);
            //Set the Curl URL.
            curl_setopt($ch, CURLOPT_URL, $authUrl);
            //Set HTTP POST Request.
            curl_setopt($ch, CURLOPT_POST, TRUE);
            //Set data to POST in HTTP "POST" Operation.
            curl_setopt($ch, CURLOPT_POSTFIELDS, $paramArr);
            //CURLOPT_RETURNTRANSFER- TRUE to return the transfer as a string of the return value of curl_exec().
            curl_setopt ($ch, CURLOPT_RETURNTRANSFER, TRUE);
            //CURLOPT_SSL_VERIFYPEER- Set FALSE to stop cURL from verifying the peer's certificate.
            curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
            //Execute the  cURL session.
            $strResponse = curl_exec($ch);
            //Get the Error Code returned by Curl.
            $curlErrno = curl_errno($ch);
            if($curlErrno){
                $curlError = curl_error($ch);
                throw new Exception($curlError);
            }
            //Close the Curl Session.
            curl_close($ch);
            //Decode the returned JSON string.
            $objResponse = json_decode($strResponse);
            if ($objResponse->error){
                throw new Exception($objResponse->error_description);
            }
            return $objResponse->access_token;
        } catch (Exception $e) {
            echo "Exception-".$e->getMessage();
        }
    }
}

/*
 * Class:HTTPTranslator
 *
 * Processing the translator request.
 */
Class HTTPTranslator {
    /*
     * Create and execute the HTTP CURL request.
     *
     * @param string $url        HTTP Url.
     * @param string $authHeader Authorization Header string.
     * @param string $postData   Data to post.
     *
     * @return string.
     *
     */
    function curlRequest($url, $authHeader, $postData=''){
        //Initialize the Curl Session.
        $ch = curl_init();
        //Set the Curl url.
        curl_setopt ($ch, CURLOPT_URL, $url);
        //Set the HTTP HEADER Fields.
        curl_setopt ($ch, CURLOPT_HTTPHEADER, array($authHeader,"Content-Type: text/xml"));
        //CURLOPT_RETURNTRANSFER- TRUE to return the transfer as a string of the return value of curl_exec().
        curl_setopt ($ch, CURLOPT_RETURNTRANSFER, TRUE);
        //CURLOPT_SSL_VERIFYPEER- Set FALSE to stop cURL from verifying the peer's certificate.
        curl_setopt ($ch, CURLOPT_SSL_VERIFYPEER, False);
        if($postData) {
            //Set HTTP POST Request.
            curl_setopt($ch, CURLOPT_POST, TRUE);
            //Set data to POST in HTTP "POST" Operation.
            curl_setopt($ch, CURLOPT_POSTFIELDS, $postData);
        }
        //Execute the  cURL session.
        $curlResponse = curl_exec($ch);
        //Get the Error Code returned by Curl.
        $curlErrno = curl_errno($ch);
        if ($curlErrno) {
            $curlError = curl_error($ch);
            throw new Exception($curlError);
        }
        //Close a cURL session.
        curl_close($ch);
        return $curlResponse;
    }


    /*
     * Create Request XML Format.
     *
     * @param string $fromLanguage   Source language Code.
     * @param string $toLanguage     Target language Code.
     * @param string $category       Category.
     * @param string $contentType    Content Type.
     * @param string $user           User Type.
     * @param string $inputStrArr    Input String Array.
     * @param string $maxTranslation MaxTranslation Count.
     *
     * @return string.
     */
    function createReqXML($fromLanguage,$toLanguage,$category,$contentType,$user,$inputStrArr,$maxTranslation) {
        //Create the XML string for passing the values.
        $requestXml = '<GetTranslationsArrayRequest>';
        $requestXml .= '<AppId></AppId>';
        $requestXml .= '<From>'.$fromLanguage.'</From>';
        $requestXml .= '<Options>'.
                  '<Category xmlns="http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2">'.$category.'</Category>'.  
                 '<ContentType xmlns="http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2">'.$contentType.'</ContentType>'.
                 '<ReservedFlags xmlns="http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2"/>'.
                 '<State xmlns="http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2"/>'.  
                 '<Uri xmlns="http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2"></Uri>'. 
                 '<User xmlns="http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2">'.$user.'</User>'. 
                 '</Options>';
        $requestXml .= '<Texts>';
        foreach($inputStrArr as $str) {
            $requestXml .= '<string xmlns="http://schemas.microsoft.com/2003/10/Serialization/Arrays">'.$str.'</string>';
        }
        $requestXml .= '</Texts>';
        $requestXml .= '<To>'.$toLanguage.'</To>';
        $requestXml .= '<MaxTranslations>'.$maxTranslation.'</MaxTranslations>';
        $requestXml .= '</GetTranslationsArrayRequest>';
        return $requestXml;
    }
}

try {
    //Client ID of the application.
    $clientID       = "clientid";
    //Client Secret key of the application.
    $clientSecret = "clientsecret";
    //OAuth Url.
    $authUrl      = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13/";
    //Application Scope Url
    $scopeUrl     = "http://api.microsofttranslator.com";
    //Application grant type
    $grantType    = "client_credentials";

    //Create the AccessTokenAuthentication object.
    $authObj      = new AccessTokenAuthentication();
    //Get the Access token.
    $accessToken  = $authObj->getTokens($grantType, $scopeUrl, $clientID, $clientSecret, $authUrl);
    //Create the authorization Header string.
    $authHeader = "Authorization: Bearer ". $accessToken;

    //Set the Params.
    $fromLanguage   = "de";
    $toLanguage        = "en";
    $user            = 'Testuser';
    $category       = "general";
    $uri             = null;
    $contentType    = "text/plain";
    $maxTranslation = 5;

    //Input text Array.
    $inputStrArr = array("Wir versuchen, offensiven Wortschatz bzw.", "Techniker arbeiten, um das Problem zu beheben");

    //HTTP GetTranslationsArray Method Url.
    $getTranslationUrl = "http://api.microsofttranslator.com/V2/Http.svc/GetTranslationsArray";

    //Create the Translator Object.
    $translatorObj = new HTTPTranslator();

    //Get the Request XML Format.
    $requestXml = $translatorObj->createReqXML($fromLanguage,$toLanguage,$category,$contentType,$user,$inputStrArr,$maxTranslation);

    //Call HTTP Curl Request.
    $curlResponse = $translatorObj->curlRequest($getTranslationUrl, $authHeader, $requestXml);

    // Interprets a string of XML into an object.
    $xmlObj = simplexml_load_string($curlResponse);
    $translationResponse = $xmlObj->GetTranslationsResponse;
    $i=0;
    foreach($translationResponse as $translationArr) {
        $translationMatchArr = $translationArr->Translations->TranslationMatch;
        echo "Get Translation For <b>$inputStrArr[$i]</b>";
        echo "<table border ='2px'>";
        echo "<tr><td><b>Count</b></td><td><b>MatchDegree</b></td>
        <td><b>Rating</b></td><td><b>TranslatedText</b></td></tr>";
        foreach($translationMatchArr as $translationMatch) {
            echo "<tr><td>$translationMatch->Count</td><td>$translationMatch->MatchDegree</td><td>$translationMatch->Rating</td>
            <td>$translationMatch->TranslatedText</td></tr>";
        }
        echo "</table></br>";
        $i++;
    }
} catch (Exception $e) {
    echo "Exception: " . $e->getMessage() . PHP_EOL;
}
