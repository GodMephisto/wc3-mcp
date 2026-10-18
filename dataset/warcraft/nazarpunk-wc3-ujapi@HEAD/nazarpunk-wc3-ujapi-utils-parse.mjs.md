---
type: vendor-doc
source_repo: "https://github.com/nazarpunk/wc3-ujapi"
source_path: "utils/parse.mjs"
ref: HEAD
---

// noinspection JSUnresolvedVariable

import * as fs from 'fs'
import Database from 'better-sqlite3'
import parse, {BinaryOp, Call, EmptyLine, FourCC, Globals, Native, Type, Variable} from 'jass-to-ast'

const alias = {
    'end': 'ends'
}

const _raw2s = string =>
    string.charCodeAt(3) |
    string.charCodeAt(2) << 8 |
    string.charCodeAt(1) << 16 |
    string.charCodeAt(0) << 24

const _isPrimitive = node => node instanceof String || ['boolean', 'number', 'string'].indexOf(typeof node) >= 0

/**
 * @param s
 * @param {boolean} rawcode
 * @return {string}
 * @private
 */
const _primitive = (s, rawcode = true) => {
    if (s instanceof FourCC) return rawcode ? `FourCC('${s}'--[[${_raw2s(s)}--]])` : `${_raw2s(s)}--[[${s}--]]`
    return s
}

/**
 * @param node
 * @param {boolean} rawcode
 * @return {string}
 * @private
 */
const _arg = (node, rawcode = true) => {
    if (_isPrimitive(node)) return _primitive(node, rawcode)
    if (node instanceof BinaryOp) return `${node.left}${node.operator}${node.right}`
    return node
}

/**
 * @param node
 * @param {boolean} rawcode
 * @return {string}
 * @private
 */
const _value = (node, rawcode = true) => {
    if (_isPrimitive(node)) return _primitive(node, rawcode)
    if (node instanceof Call) {
        const args = []
        if (node.args) {
            for (const arg of node.args) {
                args.push(_arg(arg, rawcode))
            }
        }
        return `${node.name}(${args.join(', ')})`
    }

    console.log('_value:', node)
    return node
}

// typeFix
const typeFixPath = './../asset/ConvertTypeFix.lua'
fs.writeFileSync(typeFixPath, '', {flag: 'w+'})
/** @param {string} content */
const typeFixWrite = content => fs.writeFileSync(typeFixPath, content, {flag: 'a+'})

const techPath = './../jngp/tesh/data/tesh_keywords.db'
fs.copyFileSync('./../jngp/default/tesh/data/tesh_keywords.db', techPath)

const testNativeNames = []

// noinspection JSUnusedGlobalSymbols
const db = new Database(techPath, {
    fileMustExist: true,
    verbose: () => {
    },
    //verbose: console.log,
})
const st = db.prepare('insert into main.keywords (keyword_type, name, calltip, description) values (@type, @name, @calltip, @description)')

const error = []

const insert = (type, name, calltip, description) => {
    const mx = 49
    if (name.length > mx) {
        error.push(`(${name.length})${name}`)
        name = name.substring(0, mx)
    }
    return st.run({
            type: type,
            name: name,
            calltip: calltip,
            description: description
        }
    )
}

const ujapiMap = {}

/**
 * @param {string} path
 * @param {boolean} ujapi
 */
const tolua = (path, {ujapi = false} = {}) => {
    const ast = parse(fs.readFileSync(path, {encoding: 'utf8', flag: 'r'}))
    if (!ujapi) fs.writeFileSync(`${path}.lua`, '', {flag: 'w+'})

    /** @param {string} content */
    const write = content => ujapi ? null : fs.writeFileSync(`${path}.lua`, content, {flag: 'a+'})

    /**
     * @param node
     * @return boolean
     */
    const convert = node => {
        let s = ''

        if (node instanceof EmptyLine) {
            write('\n')
            return true
        }

        if (node instanceof String) {
            write(`-- ${node.trim()}\n`)
            return true
        }

        /** Variable */
        if (node instanceof Variable) {
            if (ujapi) {
                if (node.constant) {
                    insert(
                        node.name.startsWith('EVENT_') ? 'EVENT' : 'CONSTANT',
                        node.name,
                        node.name,
                        `/*UjAPI*/ constant ${node.type} ${node.name} = ${_value(node.value, false)}`
                    )
                }
                return ujapiMap[node.name] = true
            }
            const t = `---@type ${node.type}${ujapiMap[node.name] ? ' @UjAPI' : ''}`
            write(`${node.name} = ${_value(node.value)} ${t}\n`)
            if (node.value instanceof Call) {
                typeFixWrite(`${node.name} = ${_value(node.value, false)} ${t}\n`)
            }
            return true
        }

        /** Type */
        if (node instanceof Type) {
            if (ujapi) {
                let description = `/*UjAPI*/ type ${node.base} extends ${node.super}`
                if (node.comment) {
                    description += ` //${node.comment.trim()}`
                }
                insert('NATIVE_TYPE', node.base, '', description)

                return ujapiMap[node.base] = true
            }
            s = `---@class ${node.base}:${node.super}`
            const list = []
            if (ujapiMap[node.base]) {
                list.push('UjAPI')
            }
            if (node.comment) {
                list.push(node.comment.trim())
            }
            if (list.length) {
                s += ` @${list.join(' ')}`
            }
            write(`${s.trim()}\n`)
            return true
        }

        /** Native */
        if (node instanceof Native) {
            testNativeNames.push(`'${node.name}'`)

            if (node.name.startsWith('Blz')) return true

            if (ujapi) {
                let calltip = ''
                let description = `/*UjAPI*/ ${node.constant ? 'constant ' : ''}native ${node.name} takes `
                if (node.params) {
                    const list = []
                    for (const p of node.params) {
                        list.push(`${p.type} ${p.name}`)
                    }
                    const join = list.join(', ')
                    calltip += `(${join})`
                    description += join
                } else {
                    calltip = '(nothing)'
                    description += 'nothing'
                }
                const returns = ` returns ${node.returns ?? 'nothing'}`
                calltip += returns
                description += returns

                insert('NATIVE_FUNCTION', node.name, calltip, description)
                return ujapiMap[node.name] = true
            }
            if (ujapiMap[node.name]) {
                write('---@author UjAPI\n')
            }
            if (node.params) {
                for (const p of node.params) {
                    if (alias[p.name]) p.name = alias[p.name]
                    write(`---@param ${p.name} ${p.type}\n`)
                }
            }
            if (node.returns) {
                write(`---@return ${node.returns}\n`)
            }
            write(`function ${node.name} (`)
            if (node.params) {
                const list = []
                for (const p of node.params) {
                    list.push(p.name)
                }
                if (list.length) write(list.join(', '))
            }
            write(') end\n')
            return true
        }

        console.log(node)
        return false
    }

    for (const node of ast) {
        if (node instanceof Globals) {
            if (node.globals) {
                for (const global of node.globals) {
                    if (!convert(global)) break
                }
            }
            continue
        }

        if (!convert(node)) break
    }
}

tolua('./../sdk/UjAPI.j', {ujapi: true})
tolua('./../sdk/common.j')
db.close()
console.log(error)

fs.writeFileSync('./../test/native.lua',
    `local natives = {${testNativeNames.join(',')}};
for _, v in pairs(natives) do
	if _G[v] == nil then
		printc('Native function missing: ' .. v);
	end
end`, {flag: 'w+'})
