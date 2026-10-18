---
type: vendor-doc
source_repo: "https://github.com/Kingwl/w3x-google-translate"
source_path: "pnpm-lock.yaml"
ref: HEAD
---

lockfileVersion: '6.0'

devDependencies:
  '@types/ini':
    specifier: ^1.3.31
    version: 1.3.31
  '@types/node':
    specifier: ^20.1.0
    version: 20.1.0
  axios:
    specifier: ^1.4.0
    version: 1.4.0
  dotenv:
    specifier: ^16.0.3
    version: 16.0.3
  dotenv-cli:
    specifier: ^7.2.1
    version: 7.2.1
  esbuild:
    specifier: ^0.17.18
    version: 0.17.18
  ini:
    specifier: ^4.1.0
    version: 4.1.0
  prettier:
    specifier: ^2.8.8
    version: 2.8.8
  tsx:
    specifier: ^3.12.7
    version: 3.12.7
  typescript:
    specifier: ^5.0.4
    version: 5.0.4

packages:

  /@esbuild-kit/cjs-loader@2.4.2:
    resolution: {integrity: sha512-BDXFbYOJzT/NBEtp71cvsrGPwGAMGRB/349rwKuoxNSiKjPraNNnlK6MIIabViCjqZugu6j+xeMDlEkWdHHJSg==}
    dependencies:
      '@esbuild-kit/core-utils': 3.1.0
      get-tsconfig: 4.5.0
    dev: true

  /@esbuild-kit/core-utils@3.1.0:
    resolution: {integrity: sha512-Uuk8RpCg/7fdHSceR1M6XbSZFSuMrxcePFuGgyvsBn+u339dk5OeL4jv2EojwTN2st/unJGsVm4qHWjWNmJ/tw==}
    dependencies:
      esbuild: 0.17.18
      source-map-support: 0.5.21
    dev: true

  /@esbuild-kit/esm-loader@2.5.5:
    resolution: {integrity: sha512-Qwfvj/qoPbClxCRNuac1Du01r9gvNOT+pMYtJDapfB1eoGN1YlJ1BixLyL9WVENRx5RXgNLdfYdx/CuswlGhMw==}
    dependencies:
      '@esbuild-kit/core-utils': 3.1.0
      get-tsconfig: 4.5.0
    dev: true

  /@esbuild/android-arm64@0.17.18:
    resolution: {integrity: sha512-/iq0aK0eeHgSC3z55ucMAHO05OIqmQehiGay8eP5l/5l+iEr4EIbh4/MI8xD9qRFjqzgkc0JkX0LculNC9mXBw==}
    engines: {node: '>=12'}
    cpu: [arm64]
    os: [android]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/android-arm@0.17.18:
    resolution: {integrity: sha512-EmwL+vUBZJ7mhFCs5lA4ZimpUH3WMAoqvOIYhVQwdIgSpHC8ImHdsRyhHAVxpDYUSm0lWvd63z0XH1IlImS2Qw==}
    engines: {node: '>=12'}
    cpu: [arm]
    os: [android]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/android-x64@0.17.18:
    resolution: {integrity: sha512-x+0efYNBF3NPW2Xc5bFOSFW7tTXdAcpfEg2nXmxegm4mJuVeS+i109m/7HMiOQ6M12aVGGFlqJX3RhNdYM2lWg==}
    engines: {node: '>=12'}
    cpu: [x64]
    os: [android]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/darwin-arm64@0.17.18:
    resolution: {integrity: sha512-6tY+djEAdF48M1ONWnQb1C+6LiXrKjmqjzPNPWXhu/GzOHTHX2nh8Mo2ZAmBFg0kIodHhciEgUBtcYCAIjGbjQ==}
    engines: {node: '>=12'}
    cpu: [arm64]
    os: [darwin]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/darwin-x64@0.17.18:
    resolution: {integrity: sha512-Qq84ykvLvya3dO49wVC9FFCNUfSrQJLbxhoQk/TE1r6MjHo3sFF2tlJCwMjhkBVq3/ahUisj7+EpRSz0/+8+9A==}
    engines: {node: '>=12'}
    cpu: [x64]
    os: [darwin]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/freebsd-arm64@0.17.18:
    resolution: {integrity: sha512-fw/ZfxfAzuHfaQeMDhbzxp9mc+mHn1Y94VDHFHjGvt2Uxl10mT4CDavHm+/L9KG441t1QdABqkVYwakMUeyLRA==}
    engines: {node: '>=12'}
    cpu: [arm64]
    os: [freebsd]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/freebsd-x64@0.17.18:
    resolution: {integrity: sha512-FQFbRtTaEi8ZBi/A6kxOC0V0E9B/97vPdYjY9NdawyLd4Qk5VD5g2pbWN2VR1c0xhzcJm74HWpObPszWC+qTew==}
    engines: {node: '>=12'}
    cpu: [x64]
    os: [freebsd]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/linux-arm64@0.17.18:
    resolution: {integrity: sha512-R7pZvQZFOY2sxUG8P6A21eq6q+eBv7JPQYIybHVf1XkQYC+lT7nDBdC7wWKTrbvMXKRaGudp/dzZCwL/863mZQ==}
    engines: {node: '>=12'}
    cpu: [arm64]
    os: [linux]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/linux-arm@0.17.18:
    resolution: {integrity: sha512-jW+UCM40LzHcouIaqv3e/oRs0JM76JfhHjCavPxMUti7VAPh8CaGSlS7cmyrdpzSk7A+8f0hiedHqr/LMnfijg==}
    engines: {node: '>=12'}
    cpu: [arm]
    os: [linux]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/linux-ia32@0.17.18:
    resolution: {integrity: sha512-ygIMc3I7wxgXIxk6j3V00VlABIjq260i967Cp9BNAk5pOOpIXmd1RFQJQX9Io7KRsthDrQYrtcx7QCof4o3ZoQ==}
    engines: {node: '>=12'}
    cpu: [ia32]
    os: [linux]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/linux-loong64@0.17.18:
    resolution: {integrity: sha512-bvPG+MyFs5ZlwYclCG1D744oHk1Pv7j8psF5TfYx7otCVmcJsEXgFEhQkbhNW8otDHL1a2KDINW20cfCgnzgMQ==}
    engines: {node: '>=12'}
    cpu: [loong64]
    os: [linux]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/linux-mips64el@0.17.18:
    resolution: {integrity: sha512-oVqckATOAGuiUOa6wr8TXaVPSa+6IwVJrGidmNZS1cZVx0HqkTMkqFGD2HIx9H1RvOwFeWYdaYbdY6B89KUMxA==}
    engines: {node: '>=12'}
    cpu: [mips64el]
    os: [linux]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/linux-ppc64@0.17.18:
    resolution: {integrity: sha512-3dLlQO+b/LnQNxgH4l9rqa2/IwRJVN9u/bK63FhOPB4xqiRqlQAU0qDU3JJuf0BmaH0yytTBdoSBHrb2jqc5qQ==}
    engines: {node: '>=12'}
    cpu: [ppc64]
    os: [linux]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/linux-riscv64@0.17.18:
    resolution: {integrity: sha512-/x7leOyDPjZV3TcsdfrSI107zItVnsX1q2nho7hbbQoKnmoeUWjs+08rKKt4AUXju7+3aRZSsKrJtaRmsdL1xA==}
    engines: {node: '>=12'}
    cpu: [riscv64]
    os: [linux]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/linux-s390x@0.17.18:
    resolution: {integrity: sha512-cX0I8Q9xQkL/6F5zWdYmVf5JSQt+ZfZD2bJudZrWD+4mnUvoZ3TDDXtDX2mUaq6upMFv9FlfIh4Gfun0tbGzuw==}
    engines: {node: '>=12'}
    cpu: [s390x]
    os: [linux]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/linux-x64@0.17.18:
    resolution: {integrity: sha512-66RmRsPlYy4jFl0vG80GcNRdirx4nVWAzJmXkevgphP1qf4dsLQCpSKGM3DUQCojwU1hnepI63gNZdrr02wHUA==}
    engines: {node: '>=12'}
    cpu: [x64]
    os: [linux]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/netbsd-x64@0.17.18:
    resolution: {integrity: sha512-95IRY7mI2yrkLlTLb1gpDxdC5WLC5mZDi+kA9dmM5XAGxCME0F8i4bYH4jZreaJ6lIZ0B8hTrweqG1fUyW7jbg==}
    engines: {node: '>=12'}
    cpu: [x64]
    os: [netbsd]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/openbsd-x64@0.17.18:
    resolution: {integrity: sha512-WevVOgcng+8hSZ4Q3BKL3n1xTv5H6Nb53cBrtzzEjDbbnOmucEVcZeGCsCOi9bAOcDYEeBZbD2SJNBxlfP3qiA==}
    engines: {node: '>=12'}
    cpu: [x64]
    os: [openbsd]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/sunos-x64@0.17.18:
    resolution: {integrity: sha512-Rzf4QfQagnwhQXVBS3BYUlxmEbcV7MY+BH5vfDZekU5eYpcffHSyjU8T0xucKVuOcdCsMo+Ur5wmgQJH2GfNrg==}
    engines: {node: '>=12'}
    cpu: [x64]
    os: [sunos]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/win32-arm64@0.17.18:
    resolution: {integrity: sha512-Kb3Ko/KKaWhjeAm2YoT/cNZaHaD1Yk/pa3FTsmqo9uFh1D1Rfco7BBLIPdDOozrObj2sahslFuAQGvWbgWldAg==}
    engines: {node: '>=12'}
    cpu: [arm64]
    os: [win32]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/win32-ia32@0.17.18:
    resolution: {integrity: sha512-0/xUMIdkVHwkvxfbd5+lfG7mHOf2FRrxNbPiKWg9C4fFrB8H0guClmaM3BFiRUYrznVoyxTIyC/Ou2B7QQSwmw==}
    engines: {node: '>=12'}
    cpu: [ia32]
    os: [win32]
    requiresBuild: true
    dev: true
    optional: true

  /@esbuild/win32-x64@0.17.18:
    resolution: {integrity: sha512-qU25Ma1I3NqTSHJUOKi9sAH1/Mzuvlke0ioMJRthLXKm7JiSKVwFghlGbDLOO2sARECGhja4xYfRAZNPAkooYg==}
    engines: {node: '>=12'}
    cpu: [x64]
    os: [win32]
    requiresBuild: true
    dev: true
    optional: true

  /@types/ini@1.3.31:
    resolution: {integrity: sha512-8ecxxaG4AlVEM1k9+BsziMw8UsX0qy3jYI1ad/71RrDZ+rdL6aZB0wLfAuflQiDhkD5o4yJ0uPK3OSUic3fG0w==}
    dev: true

  /@types/node@20.1.0:
    resolution: {integrity: sha512-O+z53uwx64xY7D6roOi4+jApDGFg0qn6WHcxe5QeqjMaTezBO/mxdfFXIVAVVyNWKx84OmPB3L8kbVYOTeN34A==}
    dev: true

  /asynckit@0.4.0:
    resolution: {integrity: sha512-Oei9OH4tRh0YqU3GxhX79dM/mwVgvbZJaSNaRk+bshkj0S5cfHcgYakreBjrHwatXKbz+IoIdYLxrKim2MjW0Q==}
    dev: true

  /axios@1.4.0:
    resolution: {integrity: sha512-S4XCWMEmzvo64T9GfvQDOXgYRDJ/wsSZc7Jvdgx5u1sd0JwsuPLqb3SYmusag+edF6ziyMensPVqLTSc1PiSEA==}
    dependencies:
      follow-redirects: 1.15.2
      form-data: 4.0.0
      proxy-from-env: 1.1.0
    transitivePeerDependencies:
      - debug
    dev: true

  /buffer-from@1.1.2:
    resolution: {integrity: sha512-E+XQCRwSbaaiChtv6k6Dwgc+bx+Bs6vuKJHHl5kox/BaKbhiXzqQOwK4cO22yElGp2OCmjwVhT3HmxgyPGnJfQ==}
    dev: true

  /combined-stream@1.0.8:
    resolution: {integrity: sha512-FQN4MRfuJeHf7cBbBMJFXhKSDq+2kAArBlmRBvcvFE5BB1HZKXtSFASDhdlz9zOYwxh8lDdnvmMOe/+5cdoEdg==}
    engines: {node: '>= 0.8'}
    dependencies:
      delayed-stream: 1.0.0
    dev: true

  /cross-spawn@7.0.3:
    resolution: {integrity: sha512-iRDPJKUPVEND7dHPO8rkbOnPpyDygcDFtWjpeWNCgy8WP2rXcxXL8TskReQl6OrB2G7+UJrags1q15Fudc7G6w==}
    engines: {node: '>= 8'}
    dependencies:
      path-key: 3.1.1
      shebang-command: 2.0.0
      which: 2.0.2
    dev: true

  /delayed-stream@1.0.0:
    resolution: {integrity: sha512-ZySD7Nf91aLB0RxL4KGrKHBXl7Eds1DAmEdcoVawXnLD7SDhpNgtuII2aAkg7a7QS41jxPSZ17p4VdGnMHk3MQ==}
    engines: {node: '>=0.4.0'}
    dev: true

  /dotenv-cli@7.2.1:
    resolution: {integrity: sha512-ODHbGTskqRtXAzZapDPvgNuDVQApu4oKX8lZW7Y0+9hKA6le1ZJlyRS687oU9FXjOVEDU/VFV6zI125HzhM1UQ==}
    hasBin: true
    dependencies:
      cross-spawn: 7.0.3
      dotenv: 16.0.3
      dotenv-expand: 10.0.0
      minimist: 1.2.8
    dev: true

  /dotenv-expand@10.0.0:
    resolution: {integrity: sha512-GopVGCpVS1UKH75VKHGuQFqS1Gusej0z4FyQkPdwjil2gNIv+LNsqBlboOzpJFZKVT95GkCyWJbBSdFEFUWI2A==}
    engines: {node: '>=12'}
    dev: true

  /dotenv@16.0.3:
    resolution: {integrity: sha512-7GO6HghkA5fYG9TYnNxi14/7K9f5occMlp3zXAuSxn7CKCxt9xbNWG7yF8hTCSUchlfWSe3uLmlPfigevRItzQ==}
    engines: {node: '>=12'}
    dev: true

  /esbuild@0.17.18:
    resolution: {integrity: sha512-z1lix43jBs6UKjcZVKOw2xx69ffE2aG0PygLL5qJ9OS/gy0Ewd1gW/PUQIOIQGXBHWNywSc0floSKoMFF8aK2w==}
    engines: {node: '>=12'}
    hasBin: true
    requiresBuild: true
    optionalDependencies:
      '@esbuild/android-arm': 0.17.18
      '@esbuild/android-arm64': 0.17.18
      '@esbuild/android-x64': 0.17.18
      '@esbuild/darwin-arm64': 0.17.18
      '@esbuild/darwin-x64': 0.17.18
      '@esbuild/freebsd-arm64': 0.17.18
      '@esbuild/freebsd-x64': 0.17.18
      '@esbuild/linux-arm': 0.17.18
      '@esbuild/linux-arm64': 0.17.18
      '@esbuild/linux-ia32': 0.17.18
      '@esbuild/linux-loong64': 0.17.18
      '@esbuild/linux-mips64el': 0.17.18
      '@esbuild/linux-ppc64': 0.17.18
      '@esbuild/linux-riscv64': 0.17.18
      '@esbuild/linux-s390x': 0.17.18
      '@esbuild/linux-x64': 0.17.18
      '@esbuild/netbsd-x64': 0.17.18
      '@esbuild/openbsd-x64': 0.17.18
      '@esbuild/sunos-x64': 0.17.18
      '@esbuild/win32-arm64': 0.17.18
      '@esbuild/win32-ia32': 0.17.18
      '@esbuild/win32-x64': 0.17.18
    dev: true

  /follow-redirects@1.15.2:
    resolution: {integrity: sha512-VQLG33o04KaQ8uYi2tVNbdrWp1QWxNNea+nmIB4EVM28v0hmP17z7aG1+wAkNzVq4KeXTq3221ye5qTJP91JwA==}
    engines: {node: '>=4.0'}
    peerDependencies:
      debug: '*'
    peerDependenciesMeta:
      debug:
        optional: true
    dev: true

  /form-data@4.0.0:
    resolution: {integrity: sha512-ETEklSGi5t0QMZuiXoA/Q6vcnxcLQP5vdugSpuAyi6SVGi2clPPp+xgEhuMaHC+zGgn31Kd235W35f7Hykkaww==}
    engines: {node: '>= 6'}
    dependencies:
      asynckit: 0.4.0
      combined-stream: 1.0.8
      mime-types: 2.1.35
    dev: true

  /fsevents@2.3.2:
    resolution: {integrity: sha512-xiqMQR4xAeHTuB9uWm+fFRcIOgKBMiOBP+eXiyT7jsgVCq1bkVygt00oASowB7EdtpOHaaPgKt812P9ab+DDKA==}
    engines: {node: ^8.16.0 || ^10.6.0 || >=11.0.0}
    os: [darwin]
    requiresBuild: true
    dev: true
    optional: true

  /get-tsconfig@4.5.0:
    resolution: {integrity: sha512-MjhiaIWCJ1sAU4pIQ5i5OfOuHHxVo1oYeNsWTON7jxYkod8pHocXeh+SSbmu5OZZZK73B6cbJ2XADzXehLyovQ==}
    dev: true

  /ini@4.1.0:
    resolution: {integrity: sha512-HLR38RSF2iulAzc3I/sma4CoYxQP844rPYCNfzGDOHqa/YqVlwuuZgBx6M50/X8dKgzk0cm1qRg3+47mK2N+cQ==}
    engines: {node: ^14.17.0 || ^16.13.0 || >=18.0.0}
    dev: true

  /isexe@2.0.0:
    resolution: {integrity: sha512-RHxMLp9lnKHGHRng9QFhRCMbYAcVpn69smSGcq3f36xjgVVWThj4qqLbTLlq7Ssj8B+fIQ1EuCEGI2lKsyQeIw==}
    dev: true

  /mime-db@1.52.0:
    resolution: {integrity: sha512-sPU4uV7dYlvtWJxwwxHD0PuihVNiE7TyAbQ5SWxDCB9mUYvOgroQOwYQQOKPJ8CIbE+1ETVlOoK1UC2nU3gYvg==}
    engines: {node: '>= 0.6'}
    dev: true

  /mime-types@2.1.35:
    resolution: {integrity: sha512-ZDY+bPm5zTTF+YpCrAU9nK0UgICYPT0QtT1NZWFv4s++TNkcgVaT0g6+4R2uI4MjQjzysHB1zxuWL50hzaeXiw==}
    engines: {node: '>= 0.6'}
    dependencies:
      mime-db: 1.52.0
    dev: true

  /minimist@1.2.8:
    resolution: {integrity: sha512-2yyAR8qBkN3YuheJanUpWC5U3bb5osDywNB8RzDVlDwDHbocAJveqqj1u8+SVD7jkWT4yvsHCpWqqWqAxb0zCA==}
    dev: true

  /path-key@3.1.1:
    resolution: {integrity: sha512-ojmeN0qd+y0jszEtoY48r0Peq5dwMEkIlCOu6Q5f41lfkswXuKtYrhgoTpLnyIcHm24Uhqx+5Tqm2InSwLhE6Q==}
    engines: {node: '>=8'}
    dev: true

  /prettier@2.8.8:
    resolution: {integrity: sha512-tdN8qQGvNjw4CHbY+XXk0JgCXn9QiF21a55rBe5LJAU+kDyC4WQn4+awm2Xfk2lQMk5fKup9XgzTZtGkjBdP9Q==}
    engines: {node: '>=10.13.0'}
    hasBin: true
    dev: true

  /proxy-from-env@1.1.0:
    resolution: {integrity: sha512-D+zkORCbA9f1tdWRK0RaCR3GPv50cMxcrz4X8k5LTSUD1Dkw47mKJEZQNunItRTkWwgtaUSo1RVFRIG9ZXiFYg==}
    dev: true

  /shebang-command@2.0.0:
    resolution: {integrity: sha512-kHxr2zZpYtdmrN1qDjrrX/Z1rR1kG8Dx+gkpK1G4eXmvXswmcE1hTWBWYUzlraYw1/yZp6YuDY77YtvbN0dmDA==}
    engines: {node: '>=8'}
    dependencies:
      shebang-regex: 3.0.0
    dev: true

  /shebang-regex@3.0.0:
    resolution: {integrity: sha512-7++dFhtcx3353uBaq8DDR4NuxBetBzC7ZQOhmTQInHEd6bSrXdiEyzCvG07Z44UYdLShWUyXt5M/yhz8ekcb1A==}
    engines: {node: '>=8'}
    dev: true

  /source-map-support@0.5.21:
    resolution: {integrity: sha512-uBHU3L3czsIyYXKX88fdrGovxdSCoTGDRZ6SYXtSRxLZUzHg5P/66Ht6uoUlHu9EZod+inXhKo3qQgwXUT/y1w==}
    dependencies:
      buffer-from: 1.1.2
      source-map: 0.6.1
    dev: true

  /source-map@0.6.1:
    resolution: {integrity: sha512-UjgapumWlbMhkBgzT7Ykc5YXUT46F0iKu8SGXq0bcwP5dz/h0Plj6enJqjz1Zbq2l5WaqYnrVbwWOWMyF3F47g==}
    engines: {node: '>=0.10.0'}
    dev: true

  /tsx@3.12.7:
    resolution: {integrity: sha512-C2Ip+jPmqKd1GWVQDvz/Eyc6QJbGfE7NrR3fx5BpEHMZsEHoIxHL1j+lKdGobr8ovEyqeNkPLSKp6SCSOt7gmw==}
    hasBin: true
    dependencies:
      '@esbuild-kit/cjs-loader': 2.4.2
      '@esbuild-kit/core-utils': 3.1.0
      '@esbuild-kit/esm-loader': 2.5.5
    optionalDependencies:
      fsevents: 2.3.2
    dev: true

  /typescript@5.0.4:
    resolution: {integrity: sha512-cW9T5W9xY37cc+jfEnaUvX91foxtHkza3Nw3wkoF4sSlKn0MONdkdEndig/qPBWXNkmplh3NzayQzCiHM4/hqw==}
    engines: {node: '>=12.20'}
    hasBin: true
    dev: true

  /which@2.0.2:
    resolution: {integrity: sha512-BLI3Tl1TW3Pvl70l3yq3Y64i+awpwXqsGBYWkkqMtnbXgrMD+yj7rhW0kuEDxzJaYXGjEW5ogapKNMEKNMjibA==}
    engines: {node: '>= 8'}
    hasBin: true
    dependencies:
      isexe: 2.0.0
    dev: true
