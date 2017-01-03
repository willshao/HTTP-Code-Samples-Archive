<?php // 2.01.17  AZURE Text Translation API 2017 - PHP Code Example - Cognitive Services with CURL http://www.aw6.de/azure/
// Put your parameters here:
$azure_key = "KEY_1";  // !!! TODO: secret key here !!!
$fromLanguage = "en";
$toLanguage = "de";
$inputStr = "AZURE - The official documentation and examples for PHP are useless.";
// and leave the rest of the code as it is ;-)
function getToken($azure_key)
{
    $url = 'https://api.cognitive.microsoft.com/sts/v1.0/issueToken';
    $ch = curl_init();
    $data_string = json_encode('{body}');
    curl_setopt($ch, CURLOPT_POSTFIELDS, $data_string);
    curl_setopt($ch, CURLOPT_HTTPHEADER, array(
            'Content-Type: application/json',
            'Content-Length: ' . strlen($data_string),
            'Ocp-Apim-Subscription-Key: ' . $azure_key
        )
    );
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_HEADER, false);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
    $strResponse = curl_exec($ch);
    curl_close($ch);
    return $strResponse;
}
function curlRequest($url, $authHeader)
{
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_HTTPHEADER, array($authHeader, "Content-Type: text/xml"));
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
    curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, False);
    $curlResponse = curl_exec($ch);
    curl_close($ch);
    return $curlResponse;
}
$accessToken = getToken($azure_key);
$params = "text=" . urlencode($inputStr) . "&to=" . $toLanguage . "&from=" . $fromLanguage . "&appId=Bearer+" . $accessToken;
$translateUrl = "http://api.microsofttranslator.com/v2/Http.svc/Translate?$params";
$curlResponse = curlRequest($translateUrl, $authHeader);
$xmlObj = simplexml_load_string($curlResponse);
foreach ((array)$xmlObj[0] as $val) {
    $translatedStr = $val;
}
// Translation output:
echo "<p>From " . $fromLanguage . ": " . $inputStr . "<br>";
echo "To " . $toLanguage . ": " . $translatedStr . "<br>";
echo date(r) . "<p>";
?>
