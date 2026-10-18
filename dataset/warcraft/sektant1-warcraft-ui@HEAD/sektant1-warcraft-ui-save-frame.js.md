---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "save-frame.js"
ref: HEAD
---

const { writeFileSync } = require('node:fs');
module.exports = async (page) => {
  const dataUrl = await page.evaluate(async () => {
    const img = document.querySelector('.menu-button-frame');
    const c = document.createElement('canvas');
    c.width = img.naturalWidth; c.height = img.naturalHeight;
    c.getContext('2d').drawImage(img, 0, 0);
    return c.toDataURL();
  });
  const b64 = dataUrl.replace('data:image/png;base64,', '');
  writeFileSync('frame-natural.png', Buffer.from(b64, 'base64'));
  console.log('saved frame-natural.png');
};
