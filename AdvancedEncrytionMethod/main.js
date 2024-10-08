/*
    CODE FOR FOOD
    10-08-24
*/
// Lấy các input và button từ HTML
const inputText = document.getElementById('input1');
const inputKey = document.getElementById('input2');
const outputText = document.getElementById('input3');
const encipherBtn = document.getElementById('encipherBtn');
const decipherBtn = document.getElementById('decipherBtn');
const saveBtn = document.getElementById('saveBtn');

// Hàm mở rộng từ khóa sao cho bằng độ dài của chuỗi
function extendKey(text, key) {
    let extendedKey = '';
    let keyIndex = 0;
    for (let i = 0; i < text.length; i++) {
        extendedKey += key[keyIndex % key.length]; // Lặp lại key
        keyIndex++;
    }
    return extendedKey;
}

// Hàm tính giá trị dịch chuyển dựa trên key
function calculateShift(key) {
    let shift = 0;
    for (let i = 0; i < key.length; i++) {
        shift += key.charCodeAt(i);
    }
    return shift % 128; // shift nằm trong ascii(0-127)
}

// Mã hóa với bảng mã ascii
function encrypt(text, parentKey, childKey) {
    const shift = calculateShift(parentKey);
    let result = '';

    for (let i = 0; i < text.length; i++) {
        const currentChar = text[i];
        const charCode = currentChar.charCodeAt(0);

        // Trong khoảng 32-126
        if (charCode >= 32 && charCode <= 126) {
            let newCharCode = (charCode - 32 + shift) % 95 + 32; // Giới hạn phạm vi 32-126
            result += String.fromCharCode(newCharCode);
        } else {
            result += currentChar;
        }
    }
    return result;
}

// Giải mã hóa với bảng mã ascii
function decrypt(cipher, parentKey, childKey) {
    const shift = calculateShift(parentKey);
    let result = '';

    for (let i = 0; i < cipher.length; i++) {
        const currentChar = cipher[i];
        const charCode = currentChar.charCodeAt(0);

        // Mã hóa trong khoảng 32-126
        if (charCode >= 32 && charCode <= 126) {
            let newCharCode = (charCode - 32 - shift + 95) % 95 + 32; // Giới hạn phạm vi 32-126
            result += String.fromCharCode(newCharCode);
        } else {
            result += currentChar;
        }
    }
    return result;
}

// Khi người dùng nhấn nút "Encipher"
encipherBtn.addEventListener('click', () => {
    const text = inputText.value;
    const key = inputKey.value;
    if (text && key) {
        // Mở rộng từ khóa cho bằng độ dài chuỗi
        const extendedKey = extendKey(text, key);
        // Mã hóa văn bản
        const encryptedText = encrypt(text, key, extendedKey);
        outputText.value = encryptedText; // Hiển thị kết quả
    } else {
        alert('Please enter valid input and key.');
    }
});

// Khi người dùng nhấn nút "Decipher"
decipherBtn.addEventListener('click', () => {
    const text = inputText.value;
    const key = inputKey.value;
    if (text && key) {
        // Mở rộng từ khóa cho bằng độ dài chuỗi
        const extendedKey = extendKey(text, key);
        // Giải mã văn bản
        const decryptedText = decrypt(text, key, extendedKey);
        outputText.value = decryptedText; // Hiển thị kết quả
    } else {
        alert('Please enter valid input and key.');
    }
});

// Khi người dùng nhấn nút "Save file"
saveBtn.addEventListener('click', () => {
    const output = outputText.value;
    if (output) {
        const blob = new Blob([output], { type: 'text/plain' });
        const a = document.createElement('a');
        a.href = URL.createObjectURL(blob);
        a.download = 'cipher_output.txt';
        a.click();
    } else {
        alert('No output to save.');
    }
});