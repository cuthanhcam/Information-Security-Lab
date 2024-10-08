function encryptOrDecrypt(key, mode, inputText) {
    // if (key.length !== 8) {
    //     console.log(key.length);
    //     alert("Key must be 8 characters long.");
    //     return '';
    // }

    const keyUtf8 = CryptoJS.enc.Utf8.parse(key);

    if (mode === 'ENCRYPT') {
        const encrypted = CryptoJS.DES.encrypt(inputText, keyUtf8, {
            mode: CryptoJS.mode.ECB,
            padding: CryptoJS.pad.Pkcs7
        });
        return encrypted.toString();
    } else if (mode === 'DECRYPT') {
        const decrypted = CryptoJS.DES.decrypt(inputText, keyUtf8, {
            mode: CryptoJS.mode.ECB,
            padding: CryptoJS.pad.Pkcs7
        });
        return decrypted.toString(CryptoJS.enc.Utf8);
    }
}

// Event listener for the Encipher button
document.getElementById("encipherBtn").addEventListener("click", function() {
    const inputText = document.getElementById("input1").value;
    const key = document.getElementById("input2").value;
    const result = encryptOrDecrypt(key, 'ENCRYPT', inputText);
    document.getElementById("input3").value = result;
});

// Event listener for the Decipher button
document.getElementById("decipherBtn").addEventListener("click", function() {
    const encryptedText = document.getElementById("input3").value;
    const key = document.getElementById("input2").value;
    const result = encryptOrDecrypt(key, 'DECRYPT', encryptedText);
    document.getElementById("input3").value = result;
});

// Save to file function (saving as text file)
document.getElementById("saveBtn").addEventListener("click", function() {
    const outputText = document.getElementById("input3").value;
    const blob = new Blob([outputText], { type: "text/plain;charset=utf-8" });
    const link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = "output.txt";
    link.click();
});