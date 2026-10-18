---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/parsers/mdlx/model.ts"
ref: HEAD
---

import BinaryStream from '../common/binarystream';
import { decodeUtf8, encodeUtf8 } from '../common/utf8';
import TokenStream from './tokenstream';
import Extent from './extent';
import Sequence from './sequence';
import Material from './material';
import Texture from './texture';
import TextureAnimation from './textureanimation';
import Geoset from './geoset';
import GeosetAnimation from './geosetanimation';
import GenericObject from './genericobject';
import Bone from './bone';
import Light from './light';
import Helper from './helper';
import Attachment from './attachment';
import ParticleEmitter from './particleemitter';
import ParticleEmitter2 from './particleemitter2';
import ParticleEmitterPopcorn from './particleemitterpopcorn';
import RibbonEmitter from './ribbonemitter';
import Camera from './camera';
import EventObject from './eventobject';
import CollisionShape from './collisionshape';
import FaceEffect from './faceeffect';
import UnknownChunk from './unknownchunk';
import { isMdl, isMdx } from './isformat';

// 以下用于为通用方法获取正确的类型定义。
// Used below to get proper typings for the generic methods.
type MdxStaticObject = Sequence | Texture | FaceEffect;
type MdxDynamicObject = Material | TextureAnimation | Geoset | GeosetAnimation | Bone | Light | Helper | Attachment | ParticleEmitter | ParticleEmitter2 | RibbonEmitter | Camera | EventObject | CollisionShape | ParticleEmitterPopcorn;
type MdlNumberedObject = Sequence | Texture | Material | TextureAnimation;
type MdlObject = Geoset | GeosetAnimation | Bone | Light | Helper | Attachment | ParticleEmitter | ParticleEmitter2 | RibbonEmitter | Camera | EventObject | CollisionShape | FaceEffect;

/**
 * 魔兽争霸3模型。
 * 支持从二进制MDX和文本MDL文件格式加载和保存。
 */
export default class Model {
    /**
     * 魔兽争霸3：混乱之治和冰封王座为800。
     * 魔兽争霸3：重制版为>800。
     */
    version = 800;
    name = '';
    /**
     * 据我所知，这应该始终保持为空。
     */
    animationFile = '';
    extent = new Extent();
    /**
     * 这仅由已停用的Art Tools预览器使用。
     */
    blendTime = 0;
    sequences: Sequence[] = [];
    globalSequences: number[] = [];
    materials: Material[] = [];
    textures: Texture[] = [];
    textureAnimations: TextureAnimation[] = [];
    geosets: Geoset[] = [];
    geosetAnimations: GeosetAnimation[] = [];
    bones: Bone[] = [];
    lights: Light[] = [];
    helpers: Helper[] = [];
    attachments: Attachment[] = [];
    pivotPoints: Float32Array[] = [];
    particleEmitters: ParticleEmitter[] = [];
    particleEmitters2: ParticleEmitter2[] = [];
    /**
     * @since 900
     */
    particleEmittersPopcorn: ParticleEmitterPopcorn[] = [];
    ribbonEmitters: RibbonEmitter[] = [];
    cameras: Camera[] = [];
    eventObjects: EventObject[] = [];
    collisionShapes: CollisionShape[] = [];
    /**
     * @since 900
     */
    faceEffects: FaceEffect[] = [];
    /**
     * @since 900
     */
    bindPose: Float32Array[] = [];
    /**
     * MDX格式基于块结构，魔兽争霸3不介意其中存在未知块。
     * 一些第三方工具使用此功能为模型附加元数据。
     * 当遇到未知块时，它将被添加到此处。
     * 保存为MDX时将保存这些块。
     */
    unknownChunks: UnknownChunk[] = [];

    /**
     * 从MDX或MDL加载模型。
     * 自动检测格式。
     */
    load(buffer: ArrayBuffer | Uint8Array | string): void {
        if (isMdx(buffer)) {
            if (typeof buffer === 'string') {
                buffer = encodeUtf8(buffer);
            }

            this.loadMdx(buffer);
        } else if (isMdl(buffer)) {
            if (typeof buffer !== 'string') {
                buffer = decodeUtf8(buffer);
            }

            this.loadMdl(buffer);
        } else {
            throw new Error('Not a valid MDX/MDL buffer');
        }
    }

    /**
     * 从MDX加载模型。
     */
    loadMdx(buffer: ArrayBuffer | Uint8Array): void {
        const stream = new BinaryStream(buffer);
        let tag;
        let size;

        stream.skip(4); // MDLX

        while (stream.remaining > 0) {
            tag = stream.readBinary(4);
            size = stream.readUint32();

            if (tag === 'VERS') {
                this.loadVersionChunk(stream);
            } else if (tag === 'MODL') {
                this.loadModelChunk(stream);
            } else if (tag === 'SEQS') {
                this.loadStaticObjects(this.sequences, Sequence, stream, size / 132);
            } else if (tag === 'GLBS') {
                this.loadGlobalSequenceChunk(stream, size);
            } else if (tag === 'MTLS') {
                this.loadDynamicObjects(this.materials, Material, stream, size);
            } else if (tag === 'TEXS') {
                this.loadStaticObjects(this.textures, Texture, stream, size / 268);
            } else if (tag === 'TXAN') {
                this.loadDynamicObjects(this.textureAnimations, TextureAnimation, stream, size);
            } else if (tag === 'GEOS') {
                this.loadDynamicObjects(this.geosets, Geoset, stream, size);
            } else if (tag === 'GEOA') {
                this.loadDynamicObjects(this.geosetAnimations, GeosetAnimation, stream, size);
            } else if (tag === 'BONE') {
                this.loadDynamicObjects(this.bones, Bone, stream, size);
            } else if (tag === 'LITE') {
                this.loadDynamicObjects(this.lights, Light, stream, size);
            } else if (tag === 'HELP') {
                this.loadDynamicObjects(this.helpers, Helper, stream, size);
            } else if (tag === 'ATCH') {
                this.loadDynamicObjects(this.attachments, Attachment, stream, size);
            } else if (tag === 'PIVT') {
                this.loadPivotPointChunk(stream, size);
            } else if (tag === 'PREM') {
                this.loadDynamicObjects(this.particleEmitters, ParticleEmitter, stream, size);
            } else if (tag === 'PRE2') {
                this.loadDynamicObjects(this.particleEmitters2, ParticleEmitter2, stream, size);
            } else if (tag === 'CORN') {
                this.loadDynamicObjects(this.particleEmittersPopcorn, ParticleEmitterPopcorn, stream, size);
            } else if (tag === 'RIBB') {
                this.loadDynamicObjects(this.ribbonEmitters, RibbonEmitter, stream, size);
            } else if (tag === 'CAMS') {
                this.loadDynamicObjects(this.cameras, Camera, stream, size);
            } else if (tag === 'EVTS') {
                this.loadDynamicObjects(this.eventObjects, EventObject, stream, size);
            } else if (tag === 'CLID') {
                this.loadDynamicObjects(this.collisionShapes, CollisionShape, stream, size);
            } else if (tag === 'FAFX') {
                this.loadStaticObjects(this.faceEffects, FaceEffect, stream, size / 340);
            } else if (tag === 'BPOS') {
                this.loadBindPoseChunk(stream, size);
            } else {
                this.unknownChunks.push(new UnknownChunk(stream, size, tag));
            }
        }
    }

    /**
     * 加载版本信息块
     * @param stream 二进制流对象
     */
    loadVersionChunk(stream: BinaryStream): void {
        this.version = stream.readUint32();
    }

    /**
     * 加载模型信息块
     * @param stream 二进制流对象
     */
    loadModelChunk(stream: BinaryStream): void {
        this.name = stream.read(80);
        this.animationFile = stream.read(260);
        this.extent.readMdx(stream);
        this.blendTime = stream.readUint32();
    }

    /**
     * 加载静态对象数组
     * @param out 输出数组
     * @param constructor 对象构造函数
     * @param stream 二进制流对象
     * @param count 对象数量
     */
    loadStaticObjects<T extends MdxStaticObject>(out: T[], constructor: new () => T, stream: BinaryStream, count: number): void {
        for (let i = 0; i < count; i++) {
            const object = new constructor();

            object.readMdx(stream);

            out.push(object);
        }
    }

    /**
     * 加载全局序列数据块
     * @param stream 二进制流对象
     * @param size 数据块大小
     */
    loadGlobalSequenceChunk(stream: BinaryStream, size: number): void {
        for (let i = 0, l = size / 4; i < l; i++) {
            this.globalSequences.push(stream.readUint32());
        }
    }

    /**
     * 加载动态对象数组
     * @param out 输出数组
     * @param constructor 对象构造函数
     * @param stream 二进制流对象
     * @param size 数据块大小
     */
    loadDynamicObjects<T extends MdxDynamicObject>(out: T[], constructor: new () => T, stream: BinaryStream, size: number): void {
        const end = stream.index + size;

        while (stream.index < end) {
            const object = new constructor();

            object.readMdx(stream, this.version);

            out.push(object);
        }
    }

    /**
     * 加载轴心点数据块
     * @param stream 二进制流对象
     * @param size 数据块大小
     */
    loadPivotPointChunk(stream: BinaryStream, size: number): void {
        for (let i = 0, l = size / 12; i < l; i++) {
            this.pivotPoints.push(stream.readFloat32Array(3));
        }
    }

    /**
     * 加载绑定姿势数据块
     * @param stream 二进制流对象
     * @param _size 数据块大小（未使用）
     */
    loadBindPoseChunk(stream: BinaryStream, _size: number): void {
        for (let i = 0, l = stream.readUint32(); i < l; i++) {
            this.bindPose[i] = stream.readFloat32Array(12);
        }
    }

    /**
     * 将模型保存为MDX。
     */
    saveMdx(): Uint8Array {
        const stream = new BinaryStream(new ArrayBuffer(this.getByteLength()));

        stream.writeBinary('MDLX');
        this.saveVersionChunk(stream);
        this.saveModelChunk(stream);
        this.saveStaticObjectChunk(stream, 'SEQS', this.sequences, 132);
        this.saveGlobalSequenceChunk(stream);
        this.saveDynamicObjectChunk(stream, 'MTLS', this.materials);
        this.saveStaticObjectChunk(stream, 'TEXS', this.textures, 268);
        this.saveDynamicObjectChunk(stream, 'TXAN', this.textureAnimations);
        this.saveDynamicObjectChunk(stream, 'GEOS', this.geosets);
        this.saveDynamicObjectChunk(stream, 'GEOA', this.geosetAnimations);
        this.saveDynamicObjectChunk(stream, 'BONE', this.bones);
        this.saveDynamicObjectChunk(stream, 'LITE', this.lights);
        this.saveDynamicObjectChunk(stream, 'HELP', this.helpers);
        this.saveDynamicObjectChunk(stream, 'ATCH', this.attachments);
        this.savePivotPointChunk(stream);
        this.saveDynamicObjectChunk(stream, 'PREM', this.particleEmitters);
        this.saveDynamicObjectChunk(stream, 'PRE2', this.particleEmitters2);

        if (this.version > 800) {
            this.saveDynamicObjectChunk(stream, 'CORN', this.particleEmittersPopcorn);
        }

        this.saveDynamicObjectChunk(stream, 'RIBB', this.ribbonEmitters);
        this.saveDynamicObjectChunk(stream, 'CAMS', this.cameras);
        this.saveDynamicObjectChunk(stream, 'EVTS', this.eventObjects);
        this.saveDynamicObjectChunk(stream, 'CLID', this.collisionShapes);

        if (this.version > 800) {
            this.saveStaticObjectChunk(stream, 'FAFX', this.faceEffects, 340);
            this.saveBindPoseChunk(stream);
        }

        for (const chunk of this.unknownChunks) {
            chunk.writeMdx(stream);
        }

        return stream.uint8array;
    }

    /**
     * 保存版本信息块
     * @param stream 二进制流对象
     */
    saveVersionChunk(stream: BinaryStream): void {
        stream.writeBinary('VERS');
        stream.writeUint32(4);
        stream.writeUint32(this.version);
    }

    /**
     * 保存模型信息块
     * @param stream 二进制流对象
     */
    saveModelChunk(stream: BinaryStream): void {
        stream.writeBinary('MODL');
        stream.writeUint32(372);
        stream.skip(80 - stream.write(this.name));
        stream.skip(260 - stream.write(this.animationFile));
        this.extent.writeMdx(stream);
        stream.writeUint32(this.blendTime);
    }

    /**
     * 保存静态对象数据块
     * @param stream 二进制流对象
     * @param name 块名称
     * @param objects 对象数组
     * @param size 单个对象大小
     */
    saveStaticObjectChunk(stream: BinaryStream, name: string, objects: (Sequence | Texture | FaceEffect)[], size: number): void {
        if (objects.length) {
            stream.writeBinary(name);
            stream.writeUint32(objects.length * size);

            for (const object of objects) {
                object.writeMdx(stream);
            }
        }
    }

    /**
     * 保存全局序列数据块
     * @param stream 二进制流对象
     */
    saveGlobalSequenceChunk(stream: BinaryStream): void {
        if (this.globalSequences.length) {
            stream.writeBinary('GLBS');
            stream.writeUint32(this.globalSequences.length * 4);

            for (const globalSequence of this.globalSequences) {
                stream.writeUint32(globalSequence);
            }
        }
    }

    /**
     * 保存动态对象数据块
     * @param stream 二进制流对象
     * @param name 块名称
     * @param objects 对象数组
     */
    saveDynamicObjectChunk(stream: BinaryStream, name: string, objects: (Material | TextureAnimation | Geoset | GeosetAnimation | GenericObject | Camera)[]): void {
        if (objects.length) {
            stream.writeBinary(name);
            stream.writeUint32(this.getObjectsByteLength(objects));

            for (const object of objects) {
                object.writeMdx(stream, this.version);
            }
        }
    }

    /**
     * 保存轴心点数据块
     * @param stream 二进制流对象
     */
    savePivotPointChunk(stream: BinaryStream): void {
        if (this.pivotPoints.length) {
            stream.writeBinary('PIVT');
            stream.writeUint32(this.pivotPoints.length * 12);

            for (const pivotPoint of this.pivotPoints) {
                stream.writeFloat32Array(pivotPoint);
            }
        }
    }

    /**
     * 保存绑定姿势数据块
     * @param stream 二进制流对象
     */
    saveBindPoseChunk(stream: BinaryStream): void {
        if (this.bindPose.length) {
            stream.writeBinary('BPOS');
            stream.writeUint32(4 + this.bindPose.length * 48);
            stream.writeUint32(this.bindPose.length);

            for (const matrix of this.bindPose) {
                stream.writeFloat32Array(matrix);
            }
        }
    }

    /**
     * 从MDL加载模型。
     */
    loadMdl(buffer: string): void {
        let token: string;
        const stream = new TokenStream(buffer);

        while ((token = <string>stream.readToken())) {
            if (token === 'Version') {
                this.loadVersionBlock(stream);
            } else if (token === 'Model') {
                this.loadModelBlock(stream);
            } else if (token === 'Sequences') {
                this.loadNumberedObjectBlock(this.sequences, Sequence, 'Anim', stream);
            } else if (token === 'GlobalSequences') {
                this.loadGlobalSequenceBlock(stream);
            } else if (token === 'Textures') {
                this.loadNumberedObjectBlock(this.textures, Texture, 'Bitmap', stream);
            } else if (token === 'Materials') {
                this.loadNumberedObjectBlock(this.materials, Material, 'Material', stream);
            } else if (token === 'TextureAnims') {
                this.loadNumberedObjectBlock(this.textureAnimations, TextureAnimation, 'TVertexAnim', stream);
            } else if (token === 'Geoset') {
                this.loadObject(this.geosets, Geoset, stream);
            } else if (token === 'GeosetAnim') {
                this.loadObject(this.geosetAnimations, GeosetAnimation, stream);
            } else if (token === 'Bone') {
                this.loadObject(this.bones, Bone, stream);
            } else if (token === 'Light') {
                this.loadObject(this.lights, Light, stream);
            } else if (token === 'Helper') {
                this.loadObject(this.helpers, Helper, stream);
            } else if (token === 'Attachment') {
                this.loadObject(this.attachments, Attachment, stream);
            } else if (token === 'PivotPoints') {
                this.loadPivotPointBlock(stream);
            } else if (token === 'ParticleEmitter') {
                this.loadObject(this.particleEmitters, ParticleEmitter, stream);
            } else if (token === 'ParticleEmitter2') {
                this.loadObject(this.particleEmitters2, ParticleEmitter2, stream);
            } else if (token === 'ParticleEmitterPopcorn') {
                this.loadObject(this.particleEmittersPopcorn, ParticleEmitterPopcorn, stream);
            } else if (token === 'RibbonEmitter') {
                this.loadObject(this.ribbonEmitters, RibbonEmitter, stream);
            } else if (token === 'Camera') {
                this.loadObject(this.cameras, Camera, stream);
            } else if (token === 'EventObject') {
                this.loadObject(this.eventObjects, EventObject, stream);
            } else if (token === 'CollisionShape') {
                this.loadObject(this.collisionShapes, CollisionShape, stream);
            } else if (token === 'FaceFX') {
                this.loadObject(this.faceEffects, FaceEffect, stream);
            } else if (token === 'BindPose') {
                this.loadBindPoseBlock(stream);
            } else {
                throw new Error(`Unsupported block: ${token}`);
            }
        }
    }

    /**
     * 加载版本信息块（MDL格式）
     * @param stream 令牌流对象
     */
    loadVersionBlock(stream: TokenStream): void {
        for (const token of stream.readBlock()) {
            if (token === 'FormatVersion') {
                this.version = stream.readInt();
            } else {
                throw new Error(`Unknown token in Version: "${token}"`);
            }
        }
    }

    /**
     * 加载模型信息块（MDL格式）
     * @param stream 令牌流对象
     */
    loadModelBlock(stream: TokenStream): void {
        this.name = stream.read();

        for (const token of stream.readBlock()) {
            if (token.startsWith('Num')) {
                // 不关心事物的数量，数组将根据需要增长。
                // 这包括：
                //      NumGeosets
                //      NumGeosetAnims
                //      NumHelpers
                //      NumLights
                //      NumBones
                //      NumSoundEmitters (已弃用)
                //      NumAttachments
                //      NumParticleEmitters
                //      NumParticleEmitters2
                //      NumParticleEmittersPopcorn (>800)
                //      NumRibbonEmitters
                //      NumEvents
                //      NumFaceFX (>800)
                stream.read();
            } else if (token === 'BlendTime') {
                this.blendTime = stream.readInt();
            } else if (token === 'MinimumExtent') {
                stream.readVector(this.extent.min);
            } else if (token === 'MaximumExtent') {
                stream.readVector(this.extent.max);
            } else if (token === 'BoundsRadius') {
                this.extent.boundsRadius = stream.readFloat();
            } else if (token === 'AnimationFile') {
                this.animationFile = stream.read();
            } else {
                throw new Error(`Unknown token in Model: "${token}"`);
            }
        }
    }

    /**
     * 加载带编号的对象块（MDL格式）
     * @param out 输出数组
     * @param constructor 对象构造函数
     * @param name 对象类型名称
     * @param stream 令牌流对象
     */
    loadNumberedObjectBlock<T extends MdlNumberedObject>(out: T[], constructor: new () => T, name: string, stream: TokenStream): void {
        stream.read(); // 不关心数量，数组将会增长。

        for (const token of stream.readBlock()) {
            if (token === name) {
                const object = new constructor();

                object.readMdl(stream);

                out.push(object);
            } else {
                throw new Error(`Unknown token in ${name}: "${token}"`);
            }
        }
    }

    /**
     * 加载全局序列块（MDL格式）
     * @param stream 令牌流对象
     */
    loadGlobalSequenceBlock(stream: TokenStream): void {
        stream.read(); // 不关心数量，数组将会增长。

        for (const token of stream.readBlock()) {
            if (token === 'Duration') {
                this.globalSequences.push(stream.readInt());
            } else {
                throw new Error(`Unknown token in GlobalSequences: "${token}"`);
            }
        }
    }

    loadObject<T extends MdlObject>(out: T[], constructor: new () => T, stream: TokenStream): void {
        const object = new constructor();

        object.readMdl(stream);

        out.push(object);
    }

    /**
     * 加载关键点块（MDL格式）
     * @param stream 令牌流对象
     */
    loadPivotPointBlock(stream: TokenStream): void {
        const count = stream.readInt();

        stream.read(); // {

        for (let i = 0; i < count; i++) {
            this.pivotPoints.push(stream.readVector(new Float32Array(3)));
        }

        stream.read(); // }
    }

    /**
     * 加载绑定姿势块（MDL格式）
     * @param stream 令牌流对象
     */
    loadBindPoseBlock(stream: TokenStream): void {
        for (const token of stream.readBlock()) {
            if (token === 'Matrices') {
                const matrices = stream.readInt();

                stream.read(); // {

                for (let i = 0; i < matrices; i++) {
                    this.bindPose[i] = stream.readVector(new Float32Array(12));
                }

                stream.read(); // }
            } else {
                throw new Error(`Unknown token in BindPose: "${token}"`);
            }
        }
    }

    /**
     * 将模型保存为MDL。
     */
    saveMdl(): string {
        const stream = new TokenStream();

        this.saveVersionBlock(stream);
        this.saveModelBlock(stream);
        this.saveStaticObjectsBlock(stream, 'Sequences', this.sequences);
        this.saveGlobalSequenceBlock(stream);
        this.saveStaticObjectsBlock(stream, 'Textures', this.textures);
        this.saveStaticObjectsBlock(stream, 'Materials', this.materials);
        this.saveStaticObjectsBlock(stream, 'TextureAnims', this.textureAnimations);
        this.saveObjects(stream, this.geosets);
        this.saveObjects(stream, this.geosetAnimations);
        this.saveObjects(stream, this.bones);
        this.saveObjects(stream, this.lights);
        this.saveObjects(stream, this.helpers);
        this.saveObjects(stream, this.attachments);
        this.savePivotPointBlock(stream);
        this.saveObjects(stream, this.particleEmitters);
        this.saveObjects(stream, this.particleEmitters2);

        if (this.version > 800) {
            this.saveObjects(stream, this.particleEmittersPopcorn);
        }

        this.saveObjects(stream, this.ribbonEmitters);
        this.saveObjects(stream, this.cameras);
        this.saveObjects(stream, this.eventObjects);
        this.saveObjects(stream, this.collisionShapes);

        if (this.version > 800) {
            this.saveObjects(stream, this.faceEffects);
            this.saveBindPoseBlock(stream);
        }

        return stream.buffer;
    }

    /**
     * 保存版本块（MDL格式）
     * @param stream 令牌流对象
     */
    saveVersionBlock(stream: TokenStream): void {
        stream.startBlock('Version');
        stream.writeNumberAttrib('FormatVersion', this.version);
        stream.endBlock();
    }

    /**
     * 保存模型块（MDL格式）
     * @param stream 令牌流对象
     */
    saveModelBlock(stream: TokenStream): void {
        stream.startObjectBlock('Model', this.name);
        stream.writeNumberAttrib('BlendTime', this.blendTime);

        this.extent.writeMdl(stream);

        if (this.animationFile.length) {
            stream.writeStringAttrib('AnimationFile', this.animationFile);
        }

        stream.endBlock();
    }

    /**
     * 保存静态对象块（MDL格式）
     * @param stream 令牌流对象
     * @param name 对象块名称
     * @param objects 对象数组
     */
    saveStaticObjectsBlock(stream: TokenStream, name: string, objects: (Sequence | Texture | Material | TextureAnimation)[]): void {
        if (objects.length) {
            stream.startBlock(name, objects.length);

            for (const object of objects) {
                object.writeMdl(stream, this.version);
            }

            stream.endBlock();
        }
    }

    /**
     * 保存全局序列块（MDL格式）
     * @param stream 令牌流对象
     */
    saveGlobalSequenceBlock(stream: TokenStream): void {
        if (this.globalSequences.length) {
            stream.startBlock('GlobalSequences', this.globalSequences.length);

            for (const globalSequence of this.globalSequences) {
                stream.writeNumberAttrib('Duration', globalSequence);
            }

            stream.endBlock();
        }
    }

    saveObjects(stream: TokenStream, objects: (Geoset | GeosetAnimation | Bone | Light | Helper | Attachment | ParticleEmitter | ParticleEmitter2 | RibbonEmitter | Camera | EventObject | CollisionShape | FaceEffect)[]): void {
        for (const object of objects) {
            object.writeMdl(stream, this.version);
        }
    }

    /**
     * 保存关键点块（MDL格式）
     * @param stream 令牌流对象
     */
    savePivotPointBlock(stream: TokenStream): void {
        if (this.pivotPoints.length) {
            stream.startBlock('PivotPoints', this.pivotPoints.length);

            for (const pivotPoint of this.pivotPoints) {
                stream.writeVector(pivotPoint);
            }

            stream.endBlock();
        }
    }

    /**
     * 保存绑定姿势块（MDL格式）
     * @param stream 令牌流对象
     */
    saveBindPoseBlock(stream: TokenStream): void {
        if (this.bindPose.length) {
            stream.startBlock('BindPose');

            stream.startBlock('Matrices', this.bindPose.length);

            for (const matrix of this.bindPose) {
                stream.writeVector(matrix);
            }

            stream.endBlock();

            stream.endBlock();
        }
    }

    /**
     * 计算模型作为MDX的大小。
     */
    getByteLength(): number {
        let size = 396;

        size += this.getStaticObjectsChunkByteLength(this.sequences, 132);
        size += this.getStaticObjectsChunkByteLength(this.globalSequences, 4);
        size += this.getDynamicObjectsChunkByteLength(this.materials);
        size += this.getStaticObjectsChunkByteLength(this.textures, 268);
        size += this.getDynamicObjectsChunkByteLength(this.textureAnimations);
        size += this.getDynamicObjectsChunkByteLength(this.geosets);
        size += this.getDynamicObjectsChunkByteLength(this.geosetAnimations);
        size += this.getDynamicObjectsChunkByteLength(this.bones);
        size += this.getDynamicObjectsChunkByteLength(this.lights);
        size += this.getDynamicObjectsChunkByteLength(this.helpers);
        size += this.getDynamicObjectsChunkByteLength(this.attachments);
        size += this.getStaticObjectsChunkByteLength(this.pivotPoints, 12);
        size += this.getDynamicObjectsChunkByteLength(this.particleEmitters);
        size += this.getDynamicObjectsChunkByteLength(this.particleEmitters2);

        if (this.version > 800) {
            size += this.getDynamicObjectsChunkByteLength(this.particleEmittersPopcorn);
        }

        size += this.getDynamicObjectsChunkByteLength(this.ribbonEmitters);
        size += this.getDynamicObjectsChunkByteLength(this.cameras);
        size += this.getDynamicObjectsChunkByteLength(this.eventObjects);
        size += this.getDynamicObjectsChunkByteLength(this.collisionShapes);

        if (this.version > 800) {
            size += this.getStaticObjectsChunkByteLength(this.faceEffects, 340);
            size += this.getBindPoseChunkByteLength();
        }

        size += this.getObjectsByteLength(this.unknownChunks);

        return size;
    }

    /**
     * 计算对象数组的字节长度
     * @param objects 对象数组
     * @returns 总字节数
     */
    getObjectsByteLength(objects: (Material | TextureAnimation | Geoset | GeosetAnimation | GenericObject | Camera | UnknownChunk)[]): number {
        let size = 0;

        for (const object of objects) {
            size += object.getByteLength(this.version);
        }

        return size;
    }

    /**
     * 计算动态对象块的字节长度
     * @param objects 动态对象数组
     * @returns 包括块头的总字节数
     */
    getDynamicObjectsChunkByteLength(objects: (Material | TextureAnimation | Geoset | GeosetAnimation | GenericObject | Camera | UnknownChunk)[]): number {
        if (objects.length) {
            return 8 + this.getObjectsByteLength(objects);
        }

        return 0;
    }

    /**
     * 计算静态对象块的字节长度
     * @param objects 静态对象数组
     * @param size 每个对象的大小
     * @returns 包括块头的总字节数
     */
    getStaticObjectsChunkByteLength(objects: (Sequence | number | Texture | Float32Array | FaceEffect)[], size: number): number {
        if (objects.length) {
            return 8 + objects.length * size;
        }

        return 0;
    }

    /**
     * 计算绑定姿势块的字节长度
     * @returns 绑定姿势块的字节数
     */
    getBindPoseChunkByteLength(): number {
        if (this.bindPose.length) {
            return 12 + this.bindPose.length * 48;
        }

        return 0;
    }
}
