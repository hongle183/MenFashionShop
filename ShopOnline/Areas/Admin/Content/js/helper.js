function cleanText(text) {
    // 1. Loại bỏ các ký tự không mong muốn (khoảng từ 33-47, 58-64, 91-96, 123-126)
    text = text.replace(/[\x21-\x2F\x3A-\x40\x5B-\x60\x7B-\x7E]/g, '');

    // 2. Thay thế khoảng trắng bằng dấu gạch ngang
    text = text.replace(/\s+/g, '-');

    // 3. Loại bỏ dấu diacritics (dấu tiếng Việt)
    text = text.normalize("NFD").replace(/[\u0300-\u036f]/g, '');

    // 4. Thay thế ký tự đặc biệt đ và Đ
    text = text.replace(/đ/g, 'd').replace(/Đ/g, 'D');

    return text;
}