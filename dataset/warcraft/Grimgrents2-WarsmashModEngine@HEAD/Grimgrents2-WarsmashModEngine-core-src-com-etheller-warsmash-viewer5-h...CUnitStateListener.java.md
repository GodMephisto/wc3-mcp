---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/CUnitStateListener.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation;

import com.etheller.warsmash.util.SubscriberSetNotifier;

public interface CUnitStateListener {
	void lifeChanged(); // hp (current) changes

	void ordersChanged();

	void queueChanged();

	void rallyPointChanged();

	void waypointsChanged();

	void heroStatsChanged();

	void inventoryChanged();

	public static final class CUnitStateNotifier extends SubscriberSetNotifier<CUnitStateListener>
			implements CUnitStateListener {
		@Override
		public void lifeChanged() {
			for (final CUnitStateListener listener : set) {
				listener.lifeChanged();
			}
		}

		@Override
		public void ordersChanged() {
			for (final CUnitStateListener listener : set) {
				listener.ordersChanged();
			}
		}

		@Override
		public void queueChanged() {
			for (final CUnitStateListener listener : set) {
				listener.queueChanged();
			}
		}

		@Override
		public void rallyPointChanged() {
			for (final CUnitStateListener listener : set) {
				listener.rallyPointChanged();
			}
		}

		@Override
		public void waypointsChanged() {
			for (final CUnitStateListener listener : set) {
				listener.waypointsChanged();
			}
		}

		@Override
		public void heroStatsChanged() {
			for (final CUnitStateListener listener : set) {
				listener.heroStatsChanged();
			}
		}

		@Override
		public void inventoryChanged() {
			for (final CUnitStateListener listener : set) {
				listener.inventoryChanged();
			}
		}
	}
}
