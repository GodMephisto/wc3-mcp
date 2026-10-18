---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/Table/Table.tsx"
ref: HEAD
---

import { useCallback, useRef, useState } from "react";
import { useBlpTextures, useCanvasRenderer } from "../../utils/blpLoader";
import BlpIcon from "../BlpIcon/BlpIcon";
import Tooltip from "../Tooltip/Tooltip";
import "./style.css";

// ─── Resource icon (canvas-rendered BLP) ─────────────────────────────────────

const RESOURCE_PATHS: Record<"gold" | "lumber" | "food", string> = {
  gold: "resources/ResourceGold.blp",
  lumber: "resources/ResourceLumber.blp",
  food: "resources/ResourceSupply.blp",
};

function ResourceIconCanvas({ type }: { type: "gold" | "lumber" | "food" }) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const tex = useBlpTextures({ icon: RESOURCE_PATHS[type] });

  const draw = useCallback(
    (ctx: CanvasRenderingContext2D, w: number, h: number) => {
      if (!tex) return;
      ctx.drawImage(tex.icon, 0, 0, w, h);
    },
    [tex],
  );

  useCanvasRenderer(canvasRef, draw, [tex]);
  return <canvas ref={canvasRef} className="wc-table-res-icon" aria-hidden="true" />;
}

// ─── Public types ─────────────────────────────────────────────────────────────

export interface TableTooltip {
  /** Optional icon shown in tooltip (any React node, e.g. <BlpIcon />) */
  icon?: React.ReactNode;
  content: React.ReactNode;
}

export interface TableCellDef {
  /** Text or element displayed in the cell */
  value?: React.ReactNode;
  /** BLP path — renders a BlpIcon before the value */
  iconPath?: string;
  /** Size in px for the BlpIcon (default 20) */
  iconSize?: number;
  /** Canvas-rendered resource icon shown before the value */
  resourceIcon?: "gold" | "lumber" | "food";
  /** Tooltip shown when hovering this cell */
  tooltip?: TableTooltip;
  /** Fully custom renderer — overrides all other fields */
  render?: () => React.ReactNode;
}

export interface TableColumn {
  key: string;
  header: React.ReactNode;
  width?: number | string;
  align?: "left" | "center" | "right";
  /** Tooltip shown when hovering the column header */
  headerTooltip?: TableTooltip;
}

export interface TableRowDef {
  id?: string | number;
  /** One entry per column key */
  cells: Record<string, TableCellDef>;
  /** Tooltip shown when hovering the entire row */
  tooltip?: TableTooltip;
  highlighted?: boolean;
}

interface TableProps {
  columns: TableColumn[];
  rows: TableRowDef[];
  className?: string;
  /** Prepend a # column with 1-based row numbers */
  showRowNumbers?: boolean;
}

// ─── Internal cell renderer ───────────────────────────────────────────────────

function CellContent({ cell }: { cell: TableCellDef }) {
  if (cell.render) return <>{cell.render()}</>;

  return (
    <span className="wc-table-cell-inner">
      {cell.iconPath && (
        <BlpIcon
          path={cell.iconPath}
          size={cell.iconSize ?? 20}
          className="wc-table-blp-icon"
        />
      )}
      {cell.resourceIcon && (
        <ResourceIconCanvas type={cell.resourceIcon} />
      )}
      {cell.value != null && (
        <span className="wc-table-cell-text">{cell.value}</span>
      )}
    </span>
  );
}

// ─── Header cell with optional tooltip ───────────────────────────────────────

function HeaderCell({ col }: { col: TableColumn }) {
  const [hovered, setHovered] = useState(false);

  return (
    <th
      className="wc-table-th"
      style={{
        width: col.width,
        textAlign: col.align ?? "left",
      }}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <span className="wc-table-th-label">{col.header}</span>
      {hovered && col.headerTooltip && (
        <div className="wc-table-tooltip-wrap">
          <Tooltip icon={col.headerTooltip.icon}>
            {col.headerTooltip.content}
          </Tooltip>
        </div>
      )}
    </th>
  );
}

// ─── Data cell with optional tooltip ─────────────────────────────────────────

function DataCell({
  cell,
  align,
}: {
  cell: TableCellDef;
  align?: "left" | "center" | "right";
}) {
  const [hovered, setHovered] = useState(false);

  return (
    <td
      className="wc-table-td"
      style={{ textAlign: align ?? "left" }}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <CellContent cell={cell} />
      {hovered && cell.tooltip && (
        <div className="wc-table-tooltip-wrap">
          <Tooltip icon={cell.tooltip.icon}>{cell.tooltip.content}</Tooltip>
        </div>
      )}
    </td>
  );
}

// ─── Row with optional full-row tooltip ──────────────────────────────────────

function TableRow({
  row,
  columns,
  rowNumber,
  showRowNumbers,
}: {
  row: TableRowDef;
  columns: TableColumn[];
  rowNumber: number;
  showRowNumbers: boolean;
}) {
  const [hovered, setHovered] = useState(false);

  return (
    <tr
      className={[
        "wc-table-tr",
        row.highlighted ? "wc-table-tr--highlighted" : "",
        hovered ? "wc-table-tr--hovered" : "",
      ]
        .filter(Boolean)
        .join(" ")}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      {showRowNumbers && (
        <td className="wc-table-td wc-table-td--num">{rowNumber}</td>
      )}
      {columns.map((col) => {
        const cell = row.cells[col.key] ?? {};
        return (
          <DataCell key={col.key} cell={cell} align={col.align} />
        );
      })}
      {hovered && row.tooltip && (
        <td className="wc-table-row-tooltip-cell" aria-hidden="true">
          <div className="wc-table-tooltip-wrap wc-table-tooltip-wrap--row">
            <Tooltip icon={row.tooltip.icon}>{row.tooltip.content}</Tooltip>
          </div>
        </td>
      )}
    </tr>
  );
}

// ─── Main component ───────────────────────────────────────────────────────────

export default function Table({
  columns,
  rows,
  className = "",
  showRowNumbers = false,
}: TableProps) {
  return (
    <div className={`wc-table-wrap ${className}`}>
      <table className="wc-table" role="table">
        <thead>
          <tr className="wc-table-thead-row">
            {showRowNumbers && (
              <th className="wc-table-th wc-table-th--num">#</th>
            )}
            {columns.map((col) => (
              <HeaderCell key={col.key} col={col} />
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((row, i) => (
            <TableRow
              key={row.id ?? i}
              row={row}
              columns={columns}
              rowNumber={i + 1}
              showRowNumbers={showRowNumbers}
            />
          ))}
        </tbody>
      </table>
    </div>
  );
}
