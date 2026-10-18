---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/item.service.ts"
ref: HEAD
---

import {
  EditableItemField,
  ItemEditor,
  ItemFieldUpdate,
} from "../editors/item-editor";
import { ItemDetails, ItemSummary } from "../models/item.model";
import { MapDataStorageService } from "./map-data-storage.service";

export class ItemService {
  private readonly itemEditor = new ItemEditor();
  private readonly mapDataStorageService = new MapDataStorageService();

  public listItems(): ItemSummary[] {
    const items = this.mapDataStorageService.loadItems();
    return items.map(({ id, name }) => ({ id, name }));
  }

  public getItemById(itemId: string): ItemDetails | undefined {
    const items = this.mapDataStorageService.loadItems();
    return items.find((item) => item.id === itemId);
  }

  public setItemField(
    itemId: string,
    field: EditableItemField,
    value: string
  ): ItemFieldUpdate {
    const items = this.mapDataStorageService.loadItems();
    const item = items.find((entry) => entry.id === itemId);

    if (!item) {
      throw new Error(`Item not found: ${itemId}`);
    }

    const update = this.itemEditor.setField(item, field, value);

    if (update.oldValue !== update.newValue) {
      this.mapDataStorageService.saveItems(items);
    }

    return update;
  }
}
