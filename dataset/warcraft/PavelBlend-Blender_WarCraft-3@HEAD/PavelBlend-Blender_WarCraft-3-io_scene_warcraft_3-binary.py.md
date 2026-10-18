---
type: vendor-doc
source_repo: "https://github.com/PavelBlend/Blender_WarCraft-3"
source_path: "io_scene_warcraft_3/binary.py"
ref: HEAD
---


import struct


class Reader:
    def __init__(self, data):
        self.__data = data
        self.offset = 0

    def getf(self, format):
        size = struct.calcsize(format)
        self.offset += size
        return struct.unpack_from(format, self.__data, self.offset - size)

    def gets(self, size):
        self.offset += size
        return struct.unpack_from('<{}s'.format(size), self.__data, self.offset - size)[0].split(b'\x00')[0].decode(encoding='mbcs')

    def getid(self, requiredChunksId, debug=False):
        self.offset += 4
        chunkId = struct.unpack_from('<4s', self.__data, self.offset - 4)[0].decode(encoding='mbcs')
        if type(requiredChunksId) == str:
            requiredChunksId = [requiredChunksId, ]
        if chunkId not in requiredChunksId:
            if not debug:
                raise Exception('chunk id "{0}" not in "{1}"'.format(chunkId, requiredChunksId))
        else:
            return chunkId

    def skip(self, count):
        self.offset += count
