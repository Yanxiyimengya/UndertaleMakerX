import { __UtmxItem } from "__UTMX";

export class UtmxBaseItem extends __UtmxItem {
	set displayName(value) {
		this.DisplayName = value;
	}
	get displayName() {
		return this.DisplayName;
	}

	get usedText() {
		return this.UsedText;
	}
	set usedText(value) {
		if (typeof value === "string")
		{
			this.UsedText = [value];
		}
		else
		{
			this.UsedText = value;
		}
	}
	get droppedText() {
		return this.DroppedText;
	}
	set droppedText(value) {
		if (typeof value === "string")
		{
			this.DroppedText = [value];
		}
		else
		{
			this.DroppedText = value;
		}
	}
	get infoText() {
		return this.InfoText;
	}
	set infoText(value) {
		if (typeof value === "string")
		{
			this.InfoText = [value];
		}
		else
		{
			this.InfoText = value;
		}
	}


	removeSelf() {
		this.RemoveSelf();
	}

	getSlot() {
		return this.ItemSlot;
	}

	onUse() {
	}

	onDrop() {
	}

	onInfo() {
	}
}
