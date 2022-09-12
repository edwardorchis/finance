# -*- mode: python ; coding: utf-8 -*-


block_cipher = None


a = Analysis(
    ['FinanceServerSetup.py'],
    pathex=[],
    binaries=[],
    datas=[('../libcos.dll', '.'),
        ('../libgcc_s_sjlj-1.dll', '.'),
        ('../libstdc++-6.dll', '.'),
        ('../libwinpthread-1.dll', '.'),
        ('./image/close.png', './image')],
    hiddenimports=[],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    win_no_prefer_redirects=False,
    win_private_assemblies=False,
    cipher=block_cipher,
    noarchive=False,
)
pyz = PYZ(a.pure, a.zipped_data, cipher=block_cipher)

exe = EXE(
    pyz,
    a.scripts,
    a.binaries,
    a.zipfiles,
    a.datas,
    [],
    name='FinanceServerSetup',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    upx_exclude=[],
    runtime_tmpdir=None,
    console=False,
    icon='../favicon.ico',
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
)
