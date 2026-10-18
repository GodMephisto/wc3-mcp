---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "smpq.xml"
ref: HEAD
---

<?xml version="1.0"?>
<mime-info xmlns='http://www.freedesktop.org/standards/shared-mime-info'>
  <mime-type type="application/x-mpq">
    <comment>MPQ archive</comment>
    <generic-icon name="package-x-generic"/>
    <magic priority="40">
      <match value="0x4d50511A" type="big32" offset="0:512"/>
    </magic>
  </mime-type>
  <mime-type type="application/x-mpqe">
    <comment>MPQ Encrypted archive</comment>
    <generic-icon name="package-x-generic"/>
    <glob pattern="*.mpqe" weight="40"/>
    <glob pattern="*.MPQE" weight="40"/>
  </mime-type>
</mime-info>
